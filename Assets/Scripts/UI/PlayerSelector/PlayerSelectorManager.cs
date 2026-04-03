using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectorManager : MonoBehaviour 
{
    [Header("Data Pool")]
    [Tooltip("Arraste os ScriptableObjects de todos os aviões disponíveis aqui")]
    public AirplaneData[] availableAirplanes;

    [Header("UI Instantiation")]
    [Tooltip("O Prefab que contém o script PlayerSelector a ser gerado na tela")]
    public GameObject selectorItemPrefab;
    [Tooltip("O Elemento Pai (ex: um ScrollView/Grid Layout) onde os botões serão colocados")]
    public Transform container; 

    // Lista salva agora do Item e não do Cursor. Pode ser acessada publicamente pelos cursores para mover.
    public List<PlayerSelectorItem> spawnedItems = new List<PlayerSelectorItem>();

    [Header("Cursors Container")]
    [Tooltip("Arraste um container pai na UI (ex: o próprio panel do menu) para que os cursores instanciados entrem corretamente no Canvas!")]
    public Transform cursorsContainer;

    [HideInInspector]
    public bool isLayoutReady = false;

    private void Start()
    {
        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        PopulateSelector();
        
        // Aguarda a Unity redesenhar o Canvas inteiro e encaixar os botões perfeitamente
        yield return new WaitForEndOfFrame();
        
        isLayoutReady = true;
        Debug.Log("<color=yellow>[PlayerSelectorManager]</color> Grid renderizado! O PlayerInputManager nativo já pode spawnar os cursores.");
    }

    public void PopulateSelector()
    {
        // Prevenção inteligente: limpa possíveis filhos que estivessem lá de mock ups do inspector
        if (spawnedItems.Count == 0)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
        else 
        {
            foreach (var item in spawnedItems)
            {
                if (item != null) Destroy(item.gameObject);
            }
            spawnedItems.Clear();
        }

        // Loop principal distribuindo cópias do Prefab preenchidas com as Datas
        foreach (var data in availableAirplanes)
        {
            if (data == null) continue;

            GameObject obj = Instantiate(selectorItemPrefab, container);
            PlayerSelectorItem item = obj.GetComponent<PlayerSelectorItem>();
            
            if (item != null)
            {
                item.Setup(data);
                spawnedItems.Add(item);
            }
            else
            {
                Debug.LogError("<color=red>[PlayerSelectorManager]</color> O prefab de item (selectorItemPrefab) necessita do script PlayerSelectorItem acoplado nele!");
            }
        }
        
        Debug.Log($"<color=green>[PlayerSelectorManager]</color> Preenchido com sucesso {spawnedItems.Count} itens na UI.");
    }
}