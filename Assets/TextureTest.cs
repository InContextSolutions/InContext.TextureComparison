using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;



public class TextureTest : MonoBehaviour {

    public enum Anchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public Anchor anchor;
    public int windowID;
    public string name;

    string[] projectPaths;

    string[] resolutionPaths;
    string[] typePaths;
    string[] filePaths;
    public GUISkin myskin;
    Texture[] textures;

    Material material;

    int textureIndex = 0;
    int resolutionIndex = 0;
    int typeIndex = 0;
    int projectIndex = 0;
    
    // Use this for initialization
    void Start()
    {
        projectPaths = Directory.GetDirectories(Application.streamingAssetsPath);

        SetPaths();

        material = GetComponent<Renderer>().sharedMaterial;

        LoadTextures();

        switch (anchor)
        {
            case Anchor.TopLeft:
                break;
            case Anchor.TopRight:
                windowRect.x = Screen.width - windowRect.x - windowRect.width;
                break;
            case Anchor.BottomRight:
                windowRect.x = Screen.width - windowRect.x - windowRect.width;
                windowRect.y = Screen.height - windowRect.y - windowRect.height;
                break;
            case Anchor.BottomLeft:
                windowRect.y = Screen.height - windowRect.y - windowRect.height;
                break;
        }
    }

    public Rect windowRect = new Rect(20, 20, 300, 600);

    private void OnGUI()
    {
        GUI.skin = myskin;

        windowRect = GUILayout.Window(windowID, windowRect, DoMyWindow, name);

    }

    void DoMyWindow(int windowID)
    {
        //Project
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("<-"))
        {
            projectIndex--;
            if (projectIndex < 0)
                projectIndex = projectPaths.Length - 1;

            SetPaths();

            LoadTextures();
        }

        GUILayout.Label("Project : " + Path.GetFileName(projectPaths[projectIndex]));

        if (GUILayout.Button("->"))
        {
            projectIndex++;
            if (projectIndex >= projectPaths.Length)
                projectIndex = 0;

            SetPaths();

            LoadTextures();
        }
        GUILayout.EndHorizontal();

        //Resolution
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("<-"))
        {
            resolutionIndex--;
            if (resolutionIndex < 0)
                resolutionIndex = resolutionPaths.Length - 1;

            SetPaths();

            LoadTextures();
        }

        GUILayout.Label("Resolution : " + Path.GetFileName(resolutionPaths[resolutionIndex]));

        if (GUILayout.Button("->"))
        {
            resolutionIndex++;
            if (resolutionIndex >= resolutionPaths.Length)
                resolutionIndex = 0;

            SetPaths();

            LoadTextures();
        }
        GUILayout.EndHorizontal();

        //Type
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("<-"))
        {
            typeIndex--;
            if (typeIndex < 0)
                typeIndex = typePaths.Length - 1;

            SetPaths();

            LoadTextures();
        }

        GUILayout.Label("Type : " + Path.GetFileName(typePaths[typeIndex]));

        if (GUILayout.Button("->"))
        {
            typeIndex++;
            if (typeIndex >= typePaths.Length)
                typeIndex = 0;

            SetPaths();

            LoadTextures();
        }
        GUILayout.EndHorizontal();

        //Texture
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("<-"))
        {
            textureIndex--;
            if (textureIndex < 0)
                textureIndex = filePaths.Length - 1;

            material.mainTexture = textures[textureIndex];
        }

        if (textureIndex < filePaths.Length)
            GUILayout.Label("Texture : " + Path.GetFileName(filePaths[textureIndex]));
        else
            GUILayout.Label("No Texture in this folder.");

        if (GUILayout.Button("->"))
        {
            textureIndex++;
            if (textureIndex >= filePaths.Length)
                textureIndex = 0;

            material.mainTexture = textures[textureIndex];
        }
        GUILayout.EndHorizontal();
    }

    void LoadTextures()
    {
        textures = new Texture[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            if (filePaths[i].EndsWith(".crn"))
                textures[i] = LoadDDS.LoadTextureCRN(File.ReadAllBytes(filePaths[i]));
            else if (filePaths[i].EndsWith(".dds"))
                textures[i] = LoadDDS.LoadTextureDXT(File.ReadAllBytes(filePaths[i]));
            else
                textures[i] = LoadDDS.LoadTexturePNGorJPG(File.ReadAllBytes(filePaths[i]));

            textures[i].name = Path.GetFileName(filePaths[i]);
        }

        if (textureIndex < textures.Length)
            material.mainTexture = textures[textureIndex];
        else
            material.mainTexture = null;
    }

    void SetPaths()
    {
        resolutionPaths = Directory.GetDirectories(projectPaths[projectIndex]);
        typePaths = Directory.GetDirectories(resolutionPaths[resolutionIndex]);
        filePaths = Directory.GetFiles(typePaths[typeIndex], "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".dds") || s.EndsWith(".png") || s.EndsWith(".crn") || s.EndsWith(".jpg")).ToArray();

        System.Array.Sort(filePaths, new AlphanumComparatorFast());
        filePaths = filePaths.Reverse().ToArray();
    }
}
