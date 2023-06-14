using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStructure : MonoBehaviour {

    Transform[] Structures;
    public List<Transform> listStructures = new List<Transform>();
	// Use this for initialization
	void Start () {
        Structures = this.gameObject.GetComponentsInChildren<Transform>();
        for(int i = 1; i < Structures.Length; i++)
        {
            Structures[i].gameObject.SetActive(false);
            listStructures.Add(Structures[i]);
        }
        var randomValue = Random.Range(0, listStructures.Count - 1);
        listStructures[randomValue].gameObject.SetActive(true);
        var randomX = Random.Range(-50, 50);
        var randomY = Random.Range(-50, 50);
        listStructures[randomValue].transform.parent = null;
        listStructures[randomValue].transform.localPosition = new Vector3(randomX, randomY, transform.position.z + 150);
        iTween.ScaleFrom(listStructures[randomValue].gameObject, new Vector3(0.001f, 0.001f, 0.001f), 1f);
        listStructures.RemoveAt(randomValue);
        StartCoroutine(delaySpawnStructures());
    }

    IEnumerator delaySpawnStructures()
    {
        yield return new WaitForSeconds(15);
        var randomValue = Random.Range(0, listStructures.Count - 1);
        listStructures[randomValue].gameObject.SetActive(true);
        var randomX = Random.Range(-20, 20);
        var randomY = Random.Range(-20, 20);
        listStructures[randomValue].transform.parent = null;
        listStructures[randomValue].transform.localPosition = new Vector3(randomX, randomY, transform.position.z + 150);
        iTween.ScaleFrom(listStructures[randomValue].gameObject, new Vector3(0.001f, 0.001f, 0.001f), 1f);
        listStructures.RemoveAt(randomValue);
        if (listStructures.Count > 0)
        {
            StartCoroutine(delaySpawnStructures());
        }
    }
}
