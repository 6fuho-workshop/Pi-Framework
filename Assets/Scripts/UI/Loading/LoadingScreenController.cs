// 8/3/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreenController : MonoBehaviour
{
    private VisualElement root;
    private ProgressBar progressBar;
    private Label loadingText;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        loadingText = root.Q<Label>("loadingText");
        progressBar = root.Q<ProgressBar>("progressBar");

        // Bắt đầu cập nhật thanh tiến trình
        StartCoroutine(UpdateProgress());
    }

    private System.Collections.IEnumerator UpdateProgress()
    {
        float progress = 0f;

        while (progress < 100f)
        {
            progress += Time.deltaTime * 30f; // Tăng tiến trình
            progressBar.value = progress;
            yield return null;
        }

        // Khi hoàn tất, bạn có thể chuyển sang scene tiếp theo
        Debug.Log("Loading complete!");
    }
}
