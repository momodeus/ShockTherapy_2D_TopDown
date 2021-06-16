using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScript : GridObject
{
    public static int numSmokes = 0;
    public float lifetime;
    public SpriteRenderer spriteRenderer;

    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        numSmokes++;
        startTime = Time.time;
        Destroy(gameObject, lifetime);
    }
    
    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        Setup();
    }
    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1 - (Time.time - startTime) / lifetime);
    }

    private void OnDestroy()
    {
        numSmokes--;
    }
}
