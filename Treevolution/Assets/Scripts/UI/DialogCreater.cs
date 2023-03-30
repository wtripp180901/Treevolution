using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class DialogCreater : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject dialogPrefabSmall;
    public GameObject DialogPrefabSmall
    {
        get => dialogPrefabSmall;
        set => dialogPrefabSmall = value;
    }
    public void OpenConfirmationDialogSmall()
    {

        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Confirmation Dialog, Small, Far", "This is an example of a small dialog with only one button, placed at far interaction range", false);
    }

    // Update is called once per frame

}
