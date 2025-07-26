using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TestLevelScript : MonoBehaviour , IPointerClickHandler
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private RawImage backgroundImage;
    [SerializeField]
    private RectTransform clickableArea;
    
    private Texture2D texture; //used as storage
    private Texture2D filledTexture;
    private Texture2D empty1=null;
    private Texture2D empty2=null;
    private Texture2D empty4=null;
    private Texture2D wonTexture; //used as storage

    [SerializeField]
    private ColorPanelScript colorPanelScript;

    void OnEnable()
    {
        if (exitButton == null)
        {
            Debug.LogError("Exit button is not assigned in the inspector.");
            return;
        }
        exitButton.onClick.AddListener(OnExitButtonClicked);
        if(clickableArea==null)
        {
            Debug.LogError("Clickable area not set in the inspector.");
            return;
        }
        if (backgroundImage == null)
        {
            Debug.LogError("Background image is not assigned in the inspector.");
            return;
        }
        backgroundImage.color = Color.white;
        filledTexture = Resources.Load<Texture2D>("bee_filled"); 
        empty1 = Resources.Load<Texture2D>("bee1"); 
        empty2 = Resources.Load<Texture2D>("bee2"); 
        empty4 = Resources.Load<Texture2D>("bee4"); 
        if(empty1==null || empty2==null || empty4==null)
        {
            Debug.LogError("Empty asset!");
            return;
        }
        // prepare image
        combine();
        if (backgroundImage.texture == null)
        {
            Debug.LogError("Background image texture is not assigned or not found.");
            return;
        }
        wonTexture = Resources.Load<Texture2D>("youwon"); 

        if(colorPanelScript == null)
        {
            Debug.LogError("ColorPanelScript is not assigned in the inspector.");
            return;
        }
        colorPanelScript.Init(filledTexture);
    }

    void combine()
    {
        texture=filledTexture;
        if(empty1!=null)
            texture=OverlayTextures(texture,empty1);
        if(empty2!=null)
            texture=OverlayTextures(texture,empty2);
        if(empty4!=null)
            texture=OverlayTextures(texture,empty4);
        backgroundImage.texture = texture;
    }
    bool isOnTexture(Vector2 point, Texture2D texture)
    {
        int x = Mathf.RoundToInt(point.x);
        int y = Mathf.RoundToInt(point.y);

        // Check bounds
        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            return false;

        Color pixel = texture.GetPixel(x, y);
        
        // Return true if alpha is greater than a threshold (e.g., 0.01)
        return pixel.a > 0.01f;
    }
    Texture2D OverlayTextures(Texture2D background, Texture2D foreground, int offsetX = 0, int offsetY = 0)
    {
        // Clone the background texture
        Texture2D result = new Texture2D(background.width, background.height, TextureFormat.RGBA32, false);
        result.SetPixels(background.GetPixels());

        // Loop through foreground pixels and blend onto background
        for (int x = 0; x < foreground.width; x++)
        {
            for (int y = 0; y < foreground.height; y++)
            {
                int px = x + offsetX;
                int py = y + offsetY;

                if (px < 0 || px >= background.width || py < 0 || py >= background.height)
                    continue;

                Color bgColor = result.GetPixel(px, py);
                Color fgColor = foreground.GetPixel(x, y);
                if(fgColor.a>0.01f)
                {
                    result.SetPixel(px, py, fgColor);
                }
                // Color blended = Color.Lerp(bgColor, fgColor, fgColor.a);
                
            }
        }

        result.Apply();
        return result;
    }

    void OnDisable()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;
        // Convert screen point to local coordinates within the image
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            clickableArea,
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint))
        {
            Vector2 normalizedPoint = Rect.PointToNormalized(clickableArea.rect, localPoint);
            normalizedPoint.x *= clickableArea.rect.width;
            normalizedPoint.y *= clickableArea.rect.height;
            // Debug.Log($"Texture Pixel: {normalizedPoint}");
            // int x = Mathf.FloorToInt(normalizedPoint.x * backgroundImage.texture.width);
            // int y = Mathf.FloorToInt(normalizedPoint.y * backgroundImage.texture.height);
            int x = Mathf.FloorToInt(normalizedPoint.x);
            int y = Mathf.FloorToInt(normalizedPoint.y);
            
            Color color = filledTexture.GetPixel(x,y);

            bool is1=false;
            bool is2=false;
            bool is4=false;
            if(empty1!=null)
                is1=isOnTexture(new Vector2(x,y),empty1);
            if(empty2!=null)
                is2=isOnTexture(new Vector2(x,y),empty2);
            if(empty4!=null)
                is4=isOnTexture(new Vector2(x,y),empty4);
            
            if(color==colorPanelScript.selectedColor)
            {
                if(is1)
                    empty1=null;
                if(is2)
                    empty2=null;
                if(is4)
                    empty4=null;
                combine();
            }
        }
    }

    void Update()
    {
        if(empty1==null && empty2==null && empty4==null)
        {
            backgroundImage.texture=wonTexture;
        }
    }

    private void OnExitButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    private void OnImageClicked()
    {
        Debug.Log("Background image clicked!");
    }
}
