using Unity.VisualScripting;
using UnityEngine;

public class block : MonoBehaviour
{
  [SerializeField] private GameObject myBlock;

  public void summonBlock()
  {
    myBlock.GameObject().SetActive(true);
  }
  

}
