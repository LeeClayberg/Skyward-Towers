using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BackgroundController : MonoBehaviour
{
    public static Color lightColor;

    public GameObject lightObject;

    GameObject lightParent;
    List<Vector3> lightPositions;
    List<Color> startingColors;
    bool startedColors;

    // Start is called before the first frame update
    void Start()
    {
        lightParent = new GameObject("Light Parent");
        lightParent.transform.parent = transform.Find("BuildingCeilings");
        lightParent.transform.localPosition = Vector3.zero;
        lightParent.transform.localRotation = Quaternion.Euler(Vector3.zero);
        lightParent.transform.localScale = Vector3.one;
        lightPositions = new List<Vector3>();
        startingColors = new List<Color>();
        foreach (Vector3 v in transform.Find("BuildingCeilings").GetComponent<MeshFilter>().mesh.vertices)
        {
            lightPositions.Add(v + new Vector3(0, 0, -0.05f));
            startingColors.Add(UnityEngine.Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f));
        }
        addExtraLightSpots();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in lightParent.transform)
        {
            if (StoreController.stringLights == 0)
            {
                t.GetComponent<Light>().intensity = 0;
                t.GetComponent<Light>().range = 0;
                startedColors = false;
            }
            else
            {
                t.GetComponent<Light>().intensity = 1.5f;
                t.GetComponent<Light>().range = 0.45f;
                if (StoreController.stringLights == 1)
                {
                    t.GetComponent<Light>().color = lightColor;
                    startedColors = false;
                }
                else
                {
                    if (!startedColors)
                    {
                        t.GetComponent<Light>().color = startingColors[int.Parse(t.name)];
                    }
                    else
                    {
                        float h, s, v;
                        Color.RGBToHSV(t.GetComponent<Light>().color, out h, out s, out v);
                        h += 0.003921f;
                        if (h > 1.0f)
                        {
                            h = 0;
                        }
                        t.GetComponent<Light>().color = Color.HSVToRGB(h, s, v);
                    }
                }
            }
        }
        startedColors = StoreController.stringLights == 2;
    }

    void addExtraLightSpots()
    {
        Vector3[] vertices = transform.Find("BuildingCeilings").GetComponent<MeshFilter>().mesh.vertices;
        int[] triangles = transform.Find("BuildingCeilings").GetComponent<MeshFilter>().mesh.triangles;

        List<Tuple<Vector3, Vector3>> segments = new List<Tuple<Vector3, Vector3>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int a = 0; a < 3; a++)
            {
                Tuple<Vector3, Vector3> tuple = new Tuple<Vector3, Vector3>(vertices[triangles[i + a]], vertices[triangles[i + ((a + 1) % 3)]]);
                Tuple<Vector3, Vector3> tupleReverse = new Tuple<Vector3, Vector3>(tuple.Item2, tuple.Item1);
                if (!segments.Contains(tuple) && !segments.Contains(tupleReverse))
                {
                    segments.Add(tuple);
                }
                else
                {
                    segments.Remove(tuple);
                    segments.Remove(tupleReverse);
                }
            }
        }

        List<Vector3> verticeList = vertices.ToList();
        foreach (Tuple<Vector3, Vector3> segment in segments)
        {
            int distance = Mathf.RoundToInt(Vector3.Distance(lightParent.transform.TransformPoint(segment.Item1), lightParent.transform.TransformPoint(segment.Item2))) * 2;
            for (int b = 1; b < distance; b += 1)
            {
                Vector3 point = Vector3.Lerp(segment.Item1, segment.Item2, b / (float)distance);
                Color color = Color.Lerp(startingColors[verticeList.IndexOf(segment.Item1)], startingColors[verticeList.IndexOf(segment.Item2)], b / (float)distance);
                Vector3 adjustment = new Vector3(0, 0, -0.05f);
                if ((b + 1) % 2 == 0)
                {
                    adjustment = new Vector3(0, 0, -0.12f);
                }
                else if ((b + 2) % 4 == 0)
                {
                    adjustment = new Vector3(0, 0, -0.15f);
                }
                lightPositions.Add(point + adjustment);
                startingColors.Add(color);
            }
        }

        for (int k = 0; k < lightPositions.Count; k++)
        {
            List<Vector3> otherPoints = new List<Vector3>(lightPositions);
            otherPoints.Remove(lightPositions[k]);
            bool found = false;
            Vector3 newPoint = lightParent.transform.TransformPoint(lightPositions[k]);
            foreach (Vector3 point2 in otherPoints)
            {
                Vector3 newPoint2 = lightParent.transform.TransformPoint(point2);
                if (Math.Abs(newPoint.z - newPoint2.z) < 0.01 && Math.Abs(newPoint.x - newPoint2.x) < 0.01 && newPoint2.y > newPoint.y)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                GameObject lght = Instantiate(lightObject);
                lght.transform.parent = lightParent.transform;
                lght.transform.localPosition = lightPositions[k];
                lght.GetComponent<Light>().color = startingColors[k];
                lght.name = k.ToString();
            }
        }
    }
}
