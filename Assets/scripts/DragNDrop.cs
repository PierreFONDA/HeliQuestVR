using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    private GameObject rightHandAnchor;
    private GameObject Touchobject;
    public float rayLength = 10f;
    public LayerMask raycastLayer = Physics.DefaultRaycastLayers;

    private LineRenderer lineRenderer;

    void Start()
    {
        // Recherchez le RightHandAnchor dans la hiérarchie
        rightHandAnchor = GameObject.Find("RightHandAnchor");

        // Initialiser le LineRenderer
        lineRenderer = rightHandAnchor.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.useWorldSpace = true;

        //couleur
        // Définir la couleur du LineRenderer à vert
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        //avec matérial
        // Ajouter un matériau au LineRenderer
        Material lineMaterial = new Material(Shader.Find("Standard"));
        lineMaterial.color = Color.green;
        lineRenderer.material = lineMaterial;
    }

    void Update()
    {
        
        // Vérifiez si le RightHandAnchor est défini
        if (rightHandAnchor != null)
        {
            // Créer un rayon depuis la position de RightHandAnchor
            Ray ray = new Ray(rightHandAnchor.transform.position, rightHandAnchor.transform.forward);

            // Initialiser le RaycastHit pour stocker les informations sur l'intersection
            RaycastHit hit;
            

            // Effectuer le raycast
            if (Physics.Raycast(ray, out hit))
            {
                lineRenderer.material.color = Color.red;
                // Le rayon a touché quelque chose
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    Touchobject = hit.collider.gameObject;
                    Touchobject.transform.SetParent(rightHandAnchor.transform);
                    Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                }
                else
                {
                    Touchobject.transform.SetParent(null);
                }
                

                // Mettre à jour la position de fin du LineRenderer
                lineRenderer.SetPosition(0, rightHandAnchor.transform.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.material.color = Color.green;
                // Le rayon n'a touché rien
                // Mettre à jour la position de fin du LineRenderer à la distance maximale du rayon
                Vector3 endPosition = rightHandAnchor.transform.position + rightHandAnchor.transform.forward * rayLength;
                lineRenderer.SetPosition(0, rightHandAnchor.transform.position);
                lineRenderer.SetPosition(1, endPosition);
            }
        }
        else
        {
            Debug.LogError("RightHandAnchor not found. Make sure the object with the name 'RightHandAnchor' exists in the scene.");
        }
    }
}