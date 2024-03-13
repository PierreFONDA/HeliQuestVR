using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public GameObject Helico;
    public GameObject Carrosserie;
    public GameObject GameOver;
    public GameObject bouttonD;
    public GameObject bouttonL;
    public GameObject Gas;
    private GameObject tutoGroupObject;

    // Use this for initialization

    void Start()
    {
        tutoGroupObject = GameObject.Find("planeTutorials");

        // Ajoutez un composant Rigidbody à l'objet Helico s'il n'en a pas déjà un
        Rigidbody helicoRigidbody = Helico.GetComponent<Rigidbody>();
        if (helicoRigidbody == null)
        {
            helicoRigidbody = Helico.AddComponent<Rigidbody>();
        }

        // Activez la détection des collisions pour le Rigidbody
        helicoRigidbody.detectCollisions = true;
        helicoRigidbody.useGravity = false;
    }
    void Crash()
    {
        Carrosserie.SetActive(false);
        bouttonD.SetActive(false);
        bouttonL.SetActive(false);
        Gas.SetActive(false);
        GameOver.SetActive(true);
        tutoGroupObject.SetActive(false);
        GameObject.Find("Player").GetComponent<Deplacement>().isCrashed(true);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.transform.tag != null && (collision.gameObject.transform.tag == "terrain"))
        {
            Crash();
        }
    }
    // OnCollisionEnter est appelé lorsque cet objet entre en collision avec un autre collider
    void OnCollisionEnter(Collision collision)
    {
        // Appeler la fonction Crash
        Crash();
    }

    // Update is called once per frame
    void Update()
    {
        if (Helico.transform.position.y < -155)
        {
            Crash();
        }
    }
}
