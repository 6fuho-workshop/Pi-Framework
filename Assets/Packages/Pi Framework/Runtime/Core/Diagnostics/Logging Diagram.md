# Diagram

┌─────────────────────────────────────────────────────────────────────┐
│                             Call Site                               │
│    (Game code)  PFLog.Flow.Debug("...");  PFLog.Core.Error("...");  │
└───────────────┬───────────────────────────────┬─────────────────────┘
                │                               │
                │ (extensions)                  │ (extensions)
                ▼                               ▼
        ┌─────────────────┐              ┌─────────────────┐
        │ LoggerExtensions│              │ LoggerExtensions│
        │  Trace/Debug/…  │              │   Error/Fatal   │
        └────────┬────────┘              └────────┬────────┘
                 │ (gọi vào core API)              │ (gọi vào core API)
                 └──────────────┬──────────────────┘
                                ▼
                       ┌────────────────┐
                       │    ILogger     │  (interface)
                       │  + Logger      │  (impl)
                       └──────┬─────────┘
                              │
                              │  IsEnabled(level)  ────────────────┐
                              │                                     │
                              ▼                                     │
                  ┌─────────────────────┐                           │
                  │   LogLevelSwitch    │◄───┐                      │
                  │ (per-category bind) │    │  chọn theo prefix    │
                  └──────────┬──────────┘    │  dài nhất thắng      │
                             │               │
                             ▼               │
                      ┌─────────────┐        │
                      │  LogConfig  │────────┘
                      │ default +   │
                      │ prefix rules│
                      └─────────────┘

                              │  (qua filter rồi mới snapshot scope)
                              ▼
                    ┌────────────────────┐
                    │   ScopeProvider    │
                    │ AsyncLocal stack   │
                    │ BeginScope / Pop   │
                    └─────────┬──────────┘
                              │ (trộn scopes hiện hành)
                              ▼
                        ┌─────────────┐
                        │  LogEvent   │  (struct)
                        │ ts, level,  │
                        │ category,   │
                        │ msg, ex,scope│
                        └──────┬──────┘
                               │
                               │ (fan-out)
                               ▼
                   ┌───────────────────────────┐
                   │         Sinks[]           │
                   ├────────────┬──────────────┤
                   │            │              │
        ┌──────────▼───┐   ┌───▼─────────┐ ┌──▼───────────┐
        │ ConsoleSink  │   │ UnitySink    │ │  FileSink    │
        │ format->stdout│  │ format->ULog │ │ fmt->queue→IO│
        └───────────────┘   └─────────────┘ └──────────────┘
                               │                 │
                               ▼                 ▼
                         Unity Console       Log file(s)

                              ▲
                              │  (chỉ khi level == Fatal)
                              │
                      ┌───────┴────────┐
                      │ Factory.OnFatal│  (callback tùy chọn:
                      │   (LogEvent)   │   upload log, quit/reset…)
                      └────────────────┘


     ┌─────────────────────────────────────────────┐
     │         LoggerFactory (wiring trung tâm)    │
     │  - giữ LogConfig                            │
     │  - giữ formatter                            │
     │  - danh sách sinks                          │
     │  - Create(category) → bind LevelSwitch      │
     └─────────────────────────────────────────────┘

     ┌─────────────────────────────────────────────┐
     │            SimpleTextFormatter              │
     │  - Format(LogEvent) → 1 dòng dễ grep        │
     │  - Dùng bởi các sinks text-based            │
     └─────────────────────────────────────────────┘

Ghi chú nhanh

    Call Site → LoggerExtensions → ILogger: extensions cung cấp API tiện dụng; cuối cùng mọi log đi vào ILogger.Log(...).

    Logger ↔ LogConfig/LogLevelSwitch: Logger bind cố định tới một LogLevelSwitch theo prefix dài nhất. Đổi level runtime chỉ cần cập nhật switch.

    ScopeProvider: chỉ snapshot khi log qua filter.

    LogEvent: gói dữ liệu log; chuyển cho mọi sink.

    Sinks: độc lập; có thể thêm/bớt mà không ảnh hưởng core.

    OnFatal: hook tuỳ chọn cho hành động đặc biệt sau khi ghi.

    LoggerFactory: trung tâm cấu hình & tạo logger (formatter, sinks, config).