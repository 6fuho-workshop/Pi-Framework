// Assets/PF/Editor/Common/NoticeBox.cs
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PF.PiEditor.UI
{
    public sealed class NoticeBox : VisualElement
    {
        public enum Type { Info, Warning, Error }

        private readonly VisualElement _stripe;
        private readonly Image _icon;
        private readonly Label _emoji;
        private readonly Label _label;

        public NoticeBox(string message, Type type = Type.Info, int fontSize = 18, bool bold = true)
        {
            AddToClassList("pf-notice-box");

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.FlexStart;           // căn icon theo đỉnh text cho đẹp khi nhiều dòng
            style.paddingLeft = 8;
            style.paddingRight = 8;
            style.paddingTop = 6;
            style.paddingBottom = 6;
            style.marginTop = 6;
            style.marginBottom = 6;
            style.borderTopLeftRadius = 4;
            style.borderBottomLeftRadius = 4;
            style.borderTopRightRadius = 4;
            style.borderBottomRightRadius = 4;
            //style.gap = 8;

            bool pro = EditorGUIUtility.isProSkin;
            style.backgroundColor = pro
                ? new Color(0.17f, 0.19f, 0.21f, 0.85f)
                : new Color(0.93f, 0.95f, 0.98f, 1f);

            _stripe = new VisualElement();
            _stripe.style.width = 4;
            _stripe.style.height = Length.Percent(100);
            _stripe.style.borderTopLeftRadius = 4;
            _stripe.style.borderBottomLeftRadius = 4;
            Add(_stripe);

            // Emoji (ẩn mặc định)
            _emoji = new Label();
            _emoji.style.display = DisplayStyle.None;
            _emoji.style.marginTop = 2;
            Add(_emoji);

            // Icon ảnh / vector (ẩn mặc định)
            _icon = new Image { scaleMode = ScaleMode.ScaleToFit };
            _icon.style.display = DisplayStyle.None;
            _icon.style.marginTop = 2;
            Add(_icon);

            _label = new Label(message ?? string.Empty);
            _label.style.whiteSpace = WhiteSpace.Normal;
            _label.style.flexGrow = 1;
            _label.style.unityTextAlign = TextAnchor.UpperLeft;
            SetFont(fontSize, bold);
            Add(_label);

            SetType(type);
        }

        public NoticeBox SetText(string message)
        {
            _label.text = message ?? string.Empty;
            return this;
        }

        public NoticeBox SetFont(int size, bool bold = true)
        {
            _label.style.fontSize = size;
            _label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            return this;
        }

        public NoticeBox SetType(Type type)
        {
            var stripe = type switch
            {
                Type.Info => new Color(0.22f, 0.53f, 0.86f),
                Type.Warning => new Color(1.00f, 0.72f, 0.10f),
                _ => new Color(0.90f, 0.25f, 0.25f),
            };
            _stripe.style.backgroundColor = stripe;
            return this;
        }

        // ====== ICON APIs ======

        // Dùng emoji thay icon ảnh
        public NoticeBox UseEmoji(string emoji, int size = 20)
        {
            _emoji.text = emoji ?? "";
            _emoji.style.fontSize = size;
            _emoji.style.unityFontStyleAndWeight = FontStyle.Normal;
            _emoji.style.display = string.IsNullOrEmpty(emoji) ? DisplayStyle.None : DisplayStyle.Flex;

            _icon.style.display = DisplayStyle.None;  // ẩn ảnh nếu dùng emoji
            return this;
        }

        // Dùng Texture2D (PNG/JPG)
        public NoticeBox UseIcon(Texture2D tex, int size = 18)
        {
            _icon.image = tex;
            _icon.vectorImage = null;
            _icon.style.width = size;
            _icon.style.height = size;
            _icon.style.display = tex ? DisplayStyle.Flex : DisplayStyle.None;

            _emoji.style.display = DisplayStyle.None; // ẩn emoji nếu dùng ảnh
            return this;
        }

        // Dùng VectorImage (SVG) — cần com.unity.vectorgraphics (thường có sẵn trong Editor)
        public NoticeBox UseIcon(VectorImage vi, int size = 18)
        {
            _icon.vectorImage = vi;
            _icon.image = null;
            _icon.style.width = size;
            _icon.style.height = size;
            _icon.style.display = vi ? DisplayStyle.Flex : DisplayStyle.None;

            _emoji.style.display = DisplayStyle.None;
            return this;
        }

        // Tải icon từ Asset path (ưu tiên SVG)
        public NoticeBox UseIconFromAsset(string assetPath, int size = 18)
        {
            if (string.IsNullOrEmpty(assetPath)) return this;

            var vi = AssetDatabase.LoadAssetAtPath<VectorImage>(assetPath);
            if (vi) return UseIcon(vi, size);

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (tex) return UseIcon(tex, size);

            // Không tìm thấy -> ẩn icon
            _icon.style.display = DisplayStyle.None;
            return this;
        }

        // Tải từ Editor Default Resources (Assets/Editor Default Resources/...)
        public NoticeBox UseIconFromEditorResources(string relativePath, int size = 18)
        {
            var o = EditorGUIUtility.Load(relativePath);
            if (o is Texture2D t) return UseIcon(t, size);
            return this;
        }
    }
}