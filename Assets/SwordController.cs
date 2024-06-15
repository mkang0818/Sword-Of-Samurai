using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class SwordController : MonoBehaviour
{
    public PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Sword"))
        {
            print("Ä® Ãæµ¹");
            GameObject SparkEffect = PhotonNetwork.Instantiate("SparkEffect", transform.position, Quaternion.identity);

            StartCoroutine(DestroyAfterDelay(SparkEffect));
        }
        if (col.transform.CompareTag("Player"))
        {
            print("¸ö Ãæµ¹");
            GameObject BloodEffect = PhotonNetwork.Instantiate("BloodEffect", transform.position, Quaternion.identity);
            BloodEffect.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyAfterDelay(BloodEffect));
        }
    }

    IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(1f); // 1ÃÊ ´ë±â

        PhotonNetwork.Destroy(obj);
    }
}
