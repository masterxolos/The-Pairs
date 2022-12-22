using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class SelectedEffect : MonoBehaviour
{

    [Range(0.0001f, 10f)]
    public float speed;

    [SerializeField]
    Orb leftDown, leftUp, rightUp, rightDown;

    [SerializeField]
    Transform leftDownEdge, leftUpEdge, rightUpEdge, rightDownEdge;
    private Transform[] edgePositions;

    private void Awake()
    {
        edgePositions = new Transform[4];
        edgePositions[0] = leftDownEdge;
        edgePositions[1] = leftUpEdge;
        edgePositions[2] = rightUpEdge;
        edgePositions[3] = rightDownEdge;



    }


    private float elapsedTime = 0;
    private void Update()
    {

        elapsedTime += (Time.deltaTime * speed);

        leftDown.gameObject.transform.position
            = Vector3.Lerp(edgePositions[leftDown.currentIndex].position
            , edgePositions[leftDown.targetIndex].position, elapsedTime);

        leftUp.gameObject.transform.position
            = Vector3.Lerp(edgePositions[leftUp.currentIndex].position
            , edgePositions[leftUp.targetIndex].position, elapsedTime);

        rightUp.gameObject.transform.position
            = Vector3.Lerp(edgePositions[rightUp.currentIndex].position
            , edgePositions[rightUp.targetIndex].position, elapsedTime);

        rightDown.gameObject.transform.position
            = Vector3.Lerp(edgePositions[rightDown.currentIndex].position
            , edgePositions[rightDown.targetIndex].position, elapsedTime);

        if (elapsedTime >= 1)
        {
            leftDown.currentIndex = (leftDown.currentIndex + 1) % 4;
            leftDown.targetIndex = (leftDown.targetIndex + 1) % 4;

            leftUp.currentIndex = (leftUp.currentIndex + 1) % 4;
            leftUp.targetIndex = (leftUp.targetIndex + 1) % 4;

            rightUp.currentIndex = (rightUp.currentIndex + 1) % 4;
            rightUp.targetIndex = (rightUp.targetIndex + 1) % 4;

            rightDown.currentIndex = (rightDown.currentIndex + 1) % 4;
            rightDown.targetIndex = (rightDown.targetIndex + 1) % 4;

            elapsedTime = 0;
        }



    }
    [Serializable]
    public struct Orb
    {

        public GameObject gameObject;
        public int currentIndex;
        public int targetIndex;

    }

}
