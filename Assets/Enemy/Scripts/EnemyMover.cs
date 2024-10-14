using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] [Range(0f,5f)] float speed = 1f;

    List<Node> path = new List<Node>();

    Enemy enemy;
    GridManager gridManager;
    Pathfinder pathfinder;

    void OnEnable()
    {
        ReturnToStart();
        RecalculatePath(true);
    }

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    void RecalculatePath(bool resetPath)
    {
        Vector2Int coordinates =new Vector2Int();

        if(resetPath)
        {
            coordinates = pathfinder.StartCoordinates;
        }
        else
        {
            coordinates= gridManager.GetCoordinatesFromPosition(transform.position);
        }

        StopAllCoroutines();
        path.Clear();
        path =pathfinder.GetNewPath(coordinates);
        StartCoroutine(FollowPath());
    }

    void ReturnToStart()
    {
        transform.position= gridManager.GetPostionFromCoordinates(pathfinder.StartCoordinates);
    }

    void FinishPath()
    {
        enemy.StealGold();
        gameObject.SetActive(false);
    }

    IEnumerator FollowPath()
{
    for(int i = 1; i<path.Count; i++) {
        Vector3 startPosition = this.transform.position;
        Vector3 endPosition = gridManager.GetPostionFromCoordinates(path[i].coordinates);
        float travelPercent = 0f;
 
        float rotationDegrees = degreesToRotate(this.transform.forward, startPosition, endPosition);
 
        while (travelPercent < 1f) {
            if (travelPercent < 0.25f) {
                float rotationAngle = 4 * rotationDegrees * Time.deltaTime * speed;
                this.transform.Rotate(new Vector3(0, rotationAngle, 0));
            }
 
            travelPercent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(
                startPosition, 
                endPosition, 
                travelPercent
            );
            yield return new WaitForEndOfFrame();
        }
    }
    FinishPath();
}
 
    float degreesToRotate(Vector3 currentDir, Vector3 currentPosition, Vector3 endPosition) {
    Vector3 targetDir = endPosition - currentPosition;
    return Vector3.SignedAngle(currentDir, targetDir, Vector3.up);
}
}
