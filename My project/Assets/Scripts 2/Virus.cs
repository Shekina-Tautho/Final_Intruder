using UnityEngine;

public class Virus : MonoBehaviour
{
    public string virusType = "A";

    [Header("Receptor Attachment")]
    public bool isAttaching = false;
    public Transform targetReceptor;
    public float attachmentSpeed = 2f;
    public float attachmentDistance = 0.1f;

    void Update()
    {
        if (isAttaching && targetReceptor != null)
        {
            MoveTowardReceptor();
        }
    }

    void MoveTowardReceptor()
    {
        Vector3 targetPosition = targetReceptor.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            attachmentSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(
            transform.position,
            targetPosition
        );

        if (distance <= attachmentDistance)
        {
            transform.position = targetPosition;
            isAttaching = false;

            Debug.Log("Virus reached the receptor.");
        }
    }

    public void AttachToReceptor(Transform receptor)
    {
        targetReceptor = receptor;
        isAttaching = true;

        Debug.Log(
            "Virus moving toward receptor: " +
            receptor.name
        );
    }
}