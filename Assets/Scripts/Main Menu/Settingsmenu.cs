using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

using UnityEngine.Audio;
public class Settingsmenu : MonoBehaviour
{
    [Header("Áudio")]   //aqui fica a parte de configurações de áudio
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;


    [Header("Vídeo")]   //aqui ficam as configurações de vídeo
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    Resolution[] resolutions;
    public Volume postVolume;

    private void Start()
    {
        GetResolution();
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.SetFloat("sfxVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("musicVolume", volume);
    }

    //parte das configurações de vídeo
    private void GetResolution()
    {
        //pega a resolução
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOptions = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(resolutionOptions);
            if (resolutions[i].width == Screen.currentResolution.width &&
            resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetPreset(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetPostProcess(bool isPostProcess)
    {
        postVolume.enabled = isPostProcess;
    }



}
