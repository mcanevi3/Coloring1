using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPanelScript : MonoBehaviour
{
    public Texture2D filledTexture;
    public HashSet<Color> uniqueColors;

    public float buttonSize = 60f;
    public float spacing = 10f;
    public Color selectedColor = Color.white;

    public void Init(Texture2D filled)
    {
        filledTexture = filled;
        if (filledTexture == null)
        {
            Debug.LogError("Textures not set properly.");
            return;
        }
        InitializeUniqueColors();
    }
    void InitializeUniqueColors()
    {
        // unique colors
        Color[] pixels = filledTexture.GetPixels();
        uniqueColors = new HashSet<Color>();
        foreach (Color c in pixels)
        {
            uniqueColors.Add(c);
        }
        Debug.Log($"Found {uniqueColors.Count} unique colors.");

        // int index=0;
        // foreach (Color c in uniqueColors)
        // {
        //     Debug.Log($"i:{index} Color: {c}");
        //     index++;
        // }
        CreateColorPaletteUI();
    }
    
    void CreateColorPaletteUI()
    {
        RectTransform panelRect = GetComponent<RectTransform>();
        float totalWidth = uniqueColors.Count * (buttonSize + spacing) - spacing;
        float startX = -totalWidth / 2 + buttonSize / 2;
        int i=0;
        foreach(Color color in uniqueColors)
        {
            CreateColorButton(color, startX + i * (buttonSize + spacing));
            i++;
        }
    }

    void CreateColorButton(Color color, float xPos)
    {
        GameObject buttonGO = new GameObject("ColorButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(this.transform, false);

        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(buttonSize, buttonSize);
        rt.anchoredPosition = new Vector2(xPos, 0f);

        Image img = buttonGO.GetComponent<Image>();
        img.color = color;
        img.sprite = CreateCircleSprite(); // Optional: circle sprite generation
        img.type = Image.Type.Simple;
        img.preserveAspect = true;

        Button btn = buttonGO.GetComponent<Button>();
        btn.onClick.AddListener(() => OnColorSelected(color));
    }

    void OnColorSelected(Color color)
    {
        selectedColor = color;
        Debug.Log("Selected Color: " + color);
    }

    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(0, 0, 0, 0);
        Color filled = Color.white;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - size / 2;
                float dy = y - size / 2;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                tex.SetPixel(x, y, dist <= size / 2 ? filled : transparent);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
