using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PrototypeVisuals : MonoBehaviour
{
    private static readonly Color GroundColor =
        new Color(0.10f, 0.13f, 0.17f);

    private static readonly Color WallColor =
        new Color(0.19f, 0.23f, 0.29f);

    private static readonly Color TargetColor =
        new Color(1.00f, 0.32f, 0.31f);

    private static readonly Color GhostColor =
        new Color(0.20f, 0.88f, 1.00f, 0.42f);

    private static readonly Color AccentColor =
        new Color(0.30f, 0.90f, 1.00f, 0.95f);

    private void Awake()
    {
        StyleWorld();
        StyleHud();
    }

    private void StyleWorld()
    {
        StyleRenderer("Ground", GroundColor);
        StyleRenderer("PeekWall", WallColor);
        StyleRenderer("Target", TargetColor, true);
        StyleRenderer(
            "MovementGhost",
            GhostColor,
            false,
            true
        );
    }

    private void StyleRenderer(
        string objectName,
        Color color,
        bool emissive = false,
        bool transparent = false
    )
    {
        GameObject sceneObject = GameObject.Find(objectName);

        if (
            sceneObject == null ||
            !sceneObject.TryGetComponent(
                out Renderer objectRenderer
            )
        )
        {
            Debug.LogWarning(
                "Visual styling could not find " + objectName
            );
            return;
        }

        Material material = objectRenderer.material;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else
        {
            material.color = color;
        }

        if (
            emissive &&
            material.HasProperty("_EmissionColor")
        )
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor(
                "_EmissionColor",
                color * 1.4f
            );
        }

        if (transparent)
        {
            ConfigureTransparency(material);
            objectRenderer.shadowCastingMode =
                ShadowCastingMode.Off;
        }
    }

    private void ConfigureTransparency(Material material)
    {
        material.SetFloat("_Surface", 1f);
        material.SetFloat("_Blend", 0f);
        material.SetInt(
            "_SrcBlend",
            (int)BlendMode.SrcAlpha
        );
        material.SetInt(
            "_DstBlend",
            (int)BlendMode.OneMinusSrcAlpha
        );
        material.SetInt("_ZWrite", 0);
        material.SetOverrideTag(
            "RenderType",
            "Transparent"
        );
        material.EnableKeyword(
            "_SURFACE_TYPE_TRANSPARENT"
        );
        material.renderQueue = (int)RenderQueue.Transparent;
    }

    private void StyleHud()
    {
        Canvas canvas = GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning(
                "PrototypeVisuals must be attached to the Canvas."
            );
            return;
        }

        CanvasScaler scaler =
            GetComponent<CanvasScaler>();

        if (scaler != null)
        {
            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution =
                new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        TMP_Text statusText =
            GetComponentInChildren<TMP_Text>();

        if (statusText != null)
        {
            StyleStatusText(statusText);
            CreateStatusPanel(
                statusText.rectTransform
            );
        }

        CreateCrosshair(canvas.transform);
    }

    private void StyleStatusText(TMP_Text statusText)
    {
        RectTransform textRect =
            statusText.rectTransform;

        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(0f, 1f);
        textRect.pivot = new Vector2(0f, 1f);
        textRect.anchoredPosition =
            new Vector2(48f, -42f);
        textRect.sizeDelta =
            new Vector2(570f, 270f);

        statusText.fontSize = 32f;
        statusText.color =
            new Color(0.94f, 0.97f, 1f);
        statusText.alignment =
            TextAlignmentOptions.TopLeft;
        statusText.lineSpacing = 5f;
        statusText.raycastTarget = false;
    }

    private void CreateStatusPanel(
        RectTransform textRect
    )
    {
        Transform existing =
            transform.Find("StatusPanel");

        GameObject panelObject;

        if (existing == null)
        {
            panelObject = new GameObject(
                "StatusPanel",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            panelObject.transform.SetParent(
                transform,
                false
            );
        }
        else
        {
            panelObject = existing.gameObject;
        }

        RectTransform panelRect =
            panelObject.GetComponent<RectTransform>();

        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition =
            new Vector2(24f, -24f);
        panelRect.sizeDelta =
            new Vector2(620f, 306f);

        Image panel =
            panelObject.GetComponent<Image>();

        panel.color =
            new Color(0.025f, 0.04f, 0.07f, 0.88f);
        panel.raycastTarget = false;

        panelObject.transform.SetSiblingIndex(
            textRect.GetSiblingIndex()
        );
    }

    private void CreateCrosshair(Transform canvasTransform)
    {
        Transform existing =
            canvasTransform.Find("Crosshair");

        GameObject crosshair;

        if (existing == null)
        {
            crosshair = new GameObject(
                "Crosshair",
                typeof(RectTransform)
            );
            crosshair.transform.SetParent(
                canvasTransform,
                false
            );
        }
        else
        {
            crosshair = existing.gameObject;
        }

        RectTransform crosshairRect =
            crosshair.GetComponent<RectTransform>();

        crosshairRect.anchorMin =
            new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax =
            new Vector2(0.5f, 0.5f);
        crosshairRect.pivot =
            new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition =
            Vector2.zero;
        crosshairRect.sizeDelta =
            new Vector2(24f, 24f);

        CreateCrosshairBar(
            crosshair.transform,
            "Horizontal",
            new Vector2(22f, 2f)
        );

        CreateCrosshairBar(
            crosshair.transform,
            "Vertical",
            new Vector2(2f, 22f)
        );
    }

    private void CreateCrosshairBar(
        Transform parent,
        string barName,
        Vector2 size
    )
    {
        Transform existing = parent.Find(barName);
        GameObject bar;

        if (existing == null)
        {
            bar = new GameObject(
                barName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            bar.transform.SetParent(parent, false);
        }
        else
        {
            bar = existing.gameObject;
        }

        RectTransform barRect =
            bar.GetComponent<RectTransform>();

        barRect.anchorMin =
            new Vector2(0.5f, 0.5f);
        barRect.anchorMax =
            new Vector2(0.5f, 0.5f);
        barRect.pivot =
            new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta = size;

        Image barImage = bar.GetComponent<Image>();
        barImage.color = AccentColor;
        barImage.raycastTarget = false;
    }
}
