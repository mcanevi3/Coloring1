using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TestLevelScript : MonoBehaviour , IPointerClickHandler
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private RawImage backgroundImage;
    [SerializeField]
    private RectTransform clickableArea;
    
    private Texture2D filledTexture;
    private Texture2D wonTexture; 

    private List<string> maskNames=new List<string> {"bee1", "bee2", "bee4"};
    private List<Texture2D> maskTextures=new List<Texture2D>();
    private List<bool> maskActive=new List<bool>();
    private List<Vector2Int> maskSize=new List<Vector2Int>();
    private List<Vector2Int> maskPosition=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(0,0),new Vector2Int(0,0)};

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
        foreach (string name in maskNames)
        {
            Texture2D tex=Resources.Load<Texture2D>(name);
            maskTextures.Add(tex);
            maskSize.Add(new Vector2Int(tex.width,tex.height));
            maskActive.Add(true);
        }
        for(int i=0;i<maskTextures.Count;i++)
        {
            if(maskTextures[i]==null)
            {
                Debug.Log($"Asset missing: {maskNames[i]}");
                return;
            }
        }

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
    
    bool masksRemain()
    {
        bool maskRemaining = false;
        foreach (bool value in maskActive)
        {
            if (value)
            {
                maskRemaining = true;
                break;
            }
        }
        return maskRemaining;
    }

    void combine()
    {
        Texture2D result = new Texture2D(filledTexture.width, filledTexture.height, TextureFormat.RGBA32, false);
        result.SetPixels(filledTexture.GetPixels());

        Color maskColor;
        Vector2Int pos;
        Vector2Int size;
            for(int i=0;i<maskActive.Count;i++)
            {
                if(maskActive[i])
                {
                    pos=maskPosition[i];
                    size=maskSize[i];
                    for (int x = pos.x; x < pos.x+size.x; x++)
                    {
                        for (int y = pos.y; y < pos.y+size.y; y++)
                        {
                            maskColor=maskTextures[i].GetPixel(x,y);
                            if(maskColor.a>0.01f)
                            {
                                result.SetPixel(x, y, maskColor);
                            }
                        }
                    }
                }
            }
        result.Apply();
        backgroundImage.texture=result;
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
            int x = Mathf.FloorToInt(normalizedPoint.x);
            int y = Mathf.FloorToInt(normalizedPoint.y);
            
            Color color = filledTexture.GetPixel(x,y);

            for(int i=0;i<maskActive.Count;i++)
            {
                if(maskActive[i])
                {
                    bool onTexture=isOnTexture(new Vector2(x,y),maskTextures[i]);
                    if(onTexture)
                    {
                        if(color==colorPanelScript.selectedColor)
                        {
                            maskActive[i]=false;
                        }
                    }
                }
            }
            combine();
        }
    }

    void Update()
    {
        if(!masksRemain())
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
