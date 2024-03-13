using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Deplacement : MonoBehaviour
{
    private GameObject rightHandAnchor;
    private GameObject Touchobject;
    public GameObject Manche;//les deux commandes dde delacement principales
    public GameObject Gas;
    public GameObject Debut;
    public GameObject Helico;//Le parents de joeur et carroserie
    public GameObject HandModele;
    public GameObject Rotor;
    public GameObject ReStartButton;
    public GameObject LightCommande;
    public GameObject SpotLight;
    public GameObject helicoBox;
    public AudioSource soundMoteur;
    private Animator handAnimator;
    private Animator HelicoAnimator;
    private Quaternion NewRotation;

    public float rayLength = 10f;
    private Quaternion BaseHandRotation;
    private bool BoolTouchManche = false;//me permetde passer une unique fois dans certaine initialisations
    private bool BoolTouchGas = false;
    private bool motorActivated = false;
    private bool BoolLight = false;
    private float RotorRotate = 0f;
    private Vector3 HelicoRotateAnim = new Vector3(0f, 0f, 0f);

    private LineRenderer lineRenderer;

    // Variable nécessaire pour gérer les tutos
    private GameObject planeTutorials;
    private GameObject tuto_rotation;
    private GameObject tuto_drag_manette;
    private GameObject tuto_move;
    private GameObject tuto_object_selected;
    private GameObject tuto_demarrer;
    private GameObject tuto_manette_light;
    private GameObject tuto_manette_altitude;
    private GameObject skipTuto;
    private bool tutoFini = false;
    private bool firstTimeTouchManette = true;

    private bool isCrashed_ = false;

    private GameObject previousTouchObject;

    private System.DateTime rotationTutoTime;

    void Start()
    {
        // Recherche des différents panneaux de tutoriels
        planeTutorials = GameObject.Find("planeTutorials");

        tuto_rotation = GameObject.Find("tuto_rotation");
        tuto_rotation.SetActive(false);

        tuto_drag_manette = GameObject.Find("tuto_drag_manette");
        tuto_drag_manette.SetActive(false);

        tuto_move = GameObject.Find("tuto_move");
        tuto_move.SetActive(false);

        tuto_object_selected = GameObject.Find("tuto_object_selected");
        tuto_object_selected.SetActive(false);

        tuto_manette_light = GameObject.Find("tutoManetteLight");
        tuto_manette_light.SetActive(false);

        skipTuto = GameObject.Find("skip_tuto");

        tuto_manette_altitude = GameObject.Find("tutoManetteAltitude");
        tuto_manette_altitude.SetActive(false);

        tuto_demarrer = GameObject.Find("tuto_demarrer");



        rightHandAnchor = GameObject.Find("RightHandAnchor");

        // Initialise le LineRenderer
        lineRenderer = rightHandAnchor.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.useWorldSpace = true;
        // Avec matérial
        Material lineMaterial = new Material(Shader.Find("Standard"));
        lineMaterial.color = Color.green;
        lineRenderer.material = lineMaterial;

        // Animations
        handAnimator = HandModele.GetComponent<Animator>();
        HelicoAnimator = Helico.GetComponent<Animator>();
    }

    // Gestionnaire des panneaux qui s'occupe des tuto
    void gestionTuto()
    {
        // On affiche les panneau d'aide uniquement si on a fini le tuto
        if (tutoFini && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (Touchobject == Gas)
            {
                tuto_manette_altitude.SetActive(true);
            }
            else if (Touchobject == LightCommande)
            {
                tuto_manette_light.SetActive(true);
            }
            else
            {
                tuto_manette_altitude.SetActive(false);
                tuto_manette_light.SetActive(false);
            }
        }
        // Pour avancer dans les tutos dès interaction avec l'objet concerné
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if ((Touchobject == Manche) && tuto_drag_manette.activeSelf)
            {
                if (firstTimeTouchManette)
                {
                    lineRenderer.material.color = Color.blue;
                    tuto_drag_manette.SetActive(false);
                    tuto_rotation.SetActive(true);
                    rotationTutoTime = System.DateTime.Now;
                    firstTimeTouchManette = false;
                }
            }
            else if ((Touchobject == Debut) && tuto_demarrer.activeSelf)
            {
                tuto_demarrer.SetActive(false);
                tuto_drag_manette.SetActive(true);
            }
            else if (Touchobject == skipTuto && tuto_demarrer.activeSelf)
            {
                tutoFini = true;
                tuto_rotation.SetActive(false);
                tuto_drag_manette.SetActive(false);
                tuto_move.SetActive(false);
                tuto_object_selected.SetActive(false);
                tuto_demarrer.SetActive(false);
            }
        }

        // Avancer dans le tuto automatiquement
        if (!tutoFini && rotationTutoTime != null)
        {
            System.DateTime now = System.DateTime.Now;
            System.TimeSpan difference = now.Subtract(rotationTutoTime);
            if (difference.TotalSeconds > 15 && tuto_rotation.activeSelf)
            {
                tuto_rotation.SetActive(false);
                tuto_move.SetActive(true);
                rotationTutoTime = System.DateTime.Now;

            }
            else if (difference.TotalSeconds > 10 && tuto_move.activeSelf)
            {
                tuto_move.SetActive(false);
                tuto_object_selected.SetActive(true);
                rotationTutoTime = System.DateTime.Now;
            }
            else if (difference.TotalSeconds > 10 && tuto_object_selected.activeSelf)
            {
                tuto_object_selected.SetActive(false);
                rotationTutoTime = default(System.DateTime);
                tutoFini = true;
            }
        }
    }

    void Update()
    {
        if (rightHandAnchor != null)
        {
            // Créer un rayon depuis la position de RightHandAnchor
            Ray ray = new Ray(rightHandAnchor.transform.position, rightHandAnchor.transform.forward);
            // Initialise le RaycastHit pour stocker les informations sur l'intersection
            RaycastHit hit;

            // Effectuer le raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Le rayon a touché quelque chose
                lineRenderer.material.color = Color.red;
                Touchobject = hit.collider.gameObject;

                // Animations de la main vers les objets
                if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    if (Touchobject == Manche)
                    {
                        lineRenderer.material.color = Color.yellow;
                        handAnimator.SetInteger("ChangeAnime", 1);
                    }
                    else if (Touchobject == Gas)
                    {
                        handAnimator.SetInteger("ChangeAnime", 2);
                        lineRenderer.material.color = Color.blue;
                    }
                    else if (Touchobject == Debut)
                    {
                        handAnimator.SetInteger("ChangeAnime", 3);
                    }
                    else if (Touchobject == LightCommande)
                    {
                        handAnimator.SetInteger("ChangeAnime", 4);
                    }
                    else
                    {
                        handAnimator.SetInteger("ChangeAnime", 0);
                    }
                }

                // Le boutton est appuyer on valide l'action
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    //le rayon disparait
                    lineRenderer.enabled = false;

                    //on va vérifier quelle commande est activé
                    if (Touchobject == Manche && motorActivated)
                        BoolTouchManche = true;

                    if (Touchobject == Gas && motorActivated)
                        BoolTouchGas = true;

                    if (Touchobject == Debut && !motorActivated) 
                    {

                        motorActivated = true;

                        // Allumage des moteurs
                        soundMoteur.mute = false;

                    }

                    if (Touchobject == LightCommande && motorActivated)
                        BoolLight = true;

                    if (Touchobject == ReStartButton)
                        SceneManager.LoadScene("HelicoSimulator");

                }
                else // sinon, on réactive le rayon
                {
                    lineRenderer.enabled = true;
                }

                // Mettre à jour la position de fin du LineRenderer
                lineRenderer.SetPosition(0, rightHandAnchor.transform.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else // Aucun collider toucher
            {
                // Aucun objet n'est touché
                Touchobject = null;

                lineRenderer.material.color = Color.green;

                // Mettre à jour la position de fin du LineRenderer à la distance maximale du rayon
                Vector3 endPosition = rightHandAnchor.transform.position + rightHandAnchor.transform.forward * rayLength;
                lineRenderer.SetPosition(0, rightHandAnchor.transform.position);
                lineRenderer.SetPosition(1, endPosition);
            }

            // Si on ne sélectionne rien, on annule tout
            if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                BoolTouchManche = false;
                BoolTouchGas = false;
                BoolLight = false;
                HelicoAnimator.SetInteger("ChangeAnim", 0);
            }

            // Si l'avion n'est pas craché, on peut alors le piloter
            if (!isCrashed_)
            {
                // Rotation de la main en le mettant tout de suite dans l'intervalle [-180, 180]
                Vector3 rotateHand = entre180EtMoins180(rightHandAnchor.transform.rotation.eulerAngles);

                // Animation de décollage
                if (motorActivated)
                {
                    if (RotorRotate < 10) // Pour avoir une animation de rotation du rotor qui accélère
                    {
                        RotorRotate += 0.02f;
                        Helico.transform.position += new Vector3(0f, 0.1f, 0f);
                    }

                    // Rotation du rotor en continue
                    Rotor.transform.Rotate(new Vector3(0f, 0f, RotorRotate), Space.Self);

                }

                if (BoolLight)
                {
                    BoolTouchManche = false;
                    BoolTouchGas = false;
                    Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                    SpotLight.transform.Rotate(new Vector3(-touchPosition.y / 4f, touchPosition.x / 4f, 0f));
                }

                else if (BoolTouchGas)
                {
                    BoolTouchManche = false;

                    // Changement d'altitude grâce au manche de gaz
                    Helico.transform.position += new Vector3(0, -rotateHand.x / 150, 0);
                }

                else if (BoolTouchManche)
                {
                    // Deplacement au manche grace a la rotation de la manette
                    float rotationX = HelicoRotateAnim.x + 0.008f * (rotateHand.x - HelicoRotateAnim.x);
                    float rotationZ = HelicoRotateAnim.z + 0.008f * (rotateHand.z - HelicoRotateAnim.z);
                    if (rotationX < 10 && rotationX > -10)
                        HelicoRotateAnim.x = rotationX;

                    if (rotationZ < 10 && rotationZ > -10)
                        HelicoRotateAnim.z = rotationZ;

                    // Modification de la position de la box de l'helico
                    helicoBox.transform.position -= helicoBox.transform.right * (rotateHand.z / 50);
                    helicoBox.transform.position += helicoBox.transform.forward * (rotateHand.x / 50);

                    // Rotation avec les pédales grace au touchpad
                    Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                    HelicoRotateAnim.y += touchPosition.x / 2;

                    // Application des différentes rotations sur l'helico
                    Helico.transform.rotation = Quaternion.Euler(HelicoRotateAnim.x, HelicoRotateAnim.y, HelicoRotateAnim.z);

                    // On applique uniquement la rotation sur y à la box
                    helicoBox.transform.rotation = Quaternion.Euler(0, HelicoRotateAnim.y, 0);
                } 

                // Si on ne touche pas le manche, on remet l'hélico horizontal
                if (!BoolTouchManche)
                {
                    float rotationX = HelicoRotateAnim.x - 0.02f * (HelicoRotateAnim.x);
                    float rotationZ = HelicoRotateAnim.z - 0.02f * (HelicoRotateAnim.z);

                    if (rotationX < 10 && rotationX > -10)
                        HelicoRotateAnim.x = rotationX;

                    if (rotationZ < 10 && rotationZ > -10)
                        HelicoRotateAnim.z = rotationZ;

                    Helico.transform.rotation = Quaternion.Euler(HelicoRotateAnim.x, HelicoRotateAnim.y, HelicoRotateAnim.z);
                }

                gestionTuto();
            }
        }
    }

    // Retourne un Vector3 qui se trouve entre -180 et 180
    Vector3 entre180EtMoins180(Vector3 v)
    {
        if (v.x > 180)
        {
            v.x = -(360 - v.x);
        }

        if (v.y > 180)
        {
            v.y = -(360 - v.y);
        }

        if (v.z > 180)
        {
            v.z = -(360 - v.z);
        }

        return v;
    }

    public void isCrashed(bool isCrashed)
    {
        isCrashed_ = isCrashed;
        soundMoteur.mute = true;
    }
}
