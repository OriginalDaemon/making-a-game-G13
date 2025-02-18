using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Hotbar variables")]
    public int maxhotbarsize = 6;
    public Sprite emptyhotbar;    
    public GameObject hotbarslotprefab;
    public GameObject hotbarslotgrid;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.white;
    
    [Header("3D model variables")]
    public GameObject spawnlocation;
    

    private int currentindex = 0;
    private ItemScript[] hotbarinventory;
    private GameObject[] hotbargridchildren;

    void Start()
    {
        //  Instantiate the arrays
        hotbarinventory = new ItemScript[maxhotbarsize];
        hotbargridchildren = new GameObject[maxhotbarsize];

        // Create the gameobject version of the hotbar slots within the content grid
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbarinventory[i] = null;
            GameObject childObject = Instantiate(hotbarslotprefab);
            childObject.transform.SetParent(hotbarslotgrid.transform, false);
            childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            hotbargridchildren[i] = childObject;
        }

        // Selecting the first hotbarslot
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
    }

    void selectHotbar(int index)
    {
        // Start with setting the colors of all hotbars to deselected
        // It is inefficient, but its like max 10 elements, so who cares
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

        // Edge checks
        if (index >= maxhotbarsize)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = maxhotbarsize - 1;
        }

        // Select Hotbar
        currentindex = index;
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;

        deleteHeldObjects();

        // This instantiates the 3D model of the scriptable object
        if (hotbarinventory[currentindex] != null && hotbarinventory[currentindex].model != null)
        {
            GameObject newmodel = Instantiate(hotbarinventory[currentindex].model);
            newmodel.transform.SetParent(spawnlocation.transform, false);
        }
    }

    public bool addItemToHotbar(ItemScript newItem)
    // Adds item to hotbar, returns false if hotbar is full.
    {
        int index = 0;
        while (index < maxhotbarsize)
        {
            if (hotbarinventory[index] == null)
            {
                hotbarinventory[index] = newItem;
                hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = newItem.icon;
                selectHotbar(index); 
                return true;
            }
            index++;
        }

        return false;
    }

    public bool removeItemFromHotbar(int index)
    // Removes item from hotbar, returns false if hotbarslot is empty.
    {
        if (hotbarinventory[index] == null)
        {
            return false;
        }

        hotbarinventory[index] = null; 
        hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptyhotbar;

        if (currentindex == index)
        {
            deleteHeldObjects();
        }

        return true;
    }

    private void deleteHeldObjects()
    // Deletes held objects which are loaded in the scene
    {
        // DO NOT CHANGE! Childcount is not updated until next frame, if this were to be turned into a while loop it will ***crash the game.***
        for (int i = spawnlocation.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnlocation.transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        // Increase Hotbarslot, go right
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) 
        {
            selectHotbar(currentindex + 1);
        }
        // Decrease Hotbarslot, go left
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) 
        {
            selectHotbar(currentindex - 1);
        }

        // ------------------ NEEDS TO BE MOVED TO PLAYER CONTROLS ---------------------
        if (Input.GetKeyDown(KeyCode.Q))
        {
            removeItemFromHotbar(currentindex);
        }
        // -----------------------------------------------------------------------------
    }
}
