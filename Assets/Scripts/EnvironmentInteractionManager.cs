using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class EnvironmentInteractionManager : MonoBehaviour
{
    [SerializeField] private float force = 500.0f;

    [SerializeField] private float destroyAfter = 10.0f;

    [SerializeField] private GameObject objectToThrowPrefab;
    
    [SerializeField] private InputActionProperty throwActionProperty;

    private ActionBasedController controller;

    private void Awake()
    {
        controller = FindObjectOfType<ActionBasedController>();
        throwActionProperty.action.performed += Shoot;
    }

    private void OnDestroy()
    {
        throwActionProperty.action.performed -= Shoot;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        StartCoroutine(ShootBalls());
    }
    
    private IEnumerator ShootBalls()
    {
        for (int i = 0; i < 30; i++)
        {
            var newObject = Instantiate(objectToThrowPrefab, controller.transform.position, Quaternion.identity);
            newObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            var physics = newObject.GetComponent<Rigidbody>();
            physics.AddForce(controller.transform.forward * force);
            Destroy(newObject, destroyAfter);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
