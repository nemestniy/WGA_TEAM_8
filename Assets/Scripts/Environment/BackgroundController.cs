using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    //private Texture2D backgroundTexture;
    public Texture2D biomeTexture;

    public enum Biome
    {
        Water,
        Rocky,
        Sandy,
    }

    public Biome GetBiomeByPosition(Vector3 pos)
    {
        var renderer = gameObject.GetComponent<SpriteRenderer>();
        var rot = gameObject.transform.rotation;
        Quaternion.Inverse(rot);
        var delta = pos - gameObject.transform.position;

        Vector3 size = renderer.bounds.size;

        var rotPos = -(rot * delta);
        
        var x = (rotPos.x + size.x/2 ) / size.x;
        var y = (rotPos.y + size.y/2 ) / size.y;
        
        var c = biomeTexture.GetPixel((int) (x * biomeTexture.width), (int) (y * biomeTexture.height));
        var rockColor = new Color(151.0f / 255.0f, 1, 177.0f / 255.0f);
        var sandColor = new Color(254.0f / 255.0f, 1, 141.0f / 255.0f);
        var watrColor = new Color(9.0f / 255.0f, 176.0f / 255.0f, 1);

        var pixColor = biomeTexture.GetPixel((int) (x * biomeTexture.width), (int) (y * biomeTexture.height));
        
        var rockDist = ((Vector4)((pixColor - rockColor))).magnitude;
        var sandDist = ((Vector4)((pixColor - sandColor))).magnitude;
        var watrDist = ((Vector4)((pixColor - watrColor))).magnitude;

        if (rockDist < sandDist && rockDist < watrDist) return Biome.Rocky;
        if (sandDist < watrDist) return Biome.Sandy;
        return Biome.Water;
    }
}
