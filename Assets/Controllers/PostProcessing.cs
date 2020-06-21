using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public class PostProcessing : MonoBehaviour
{
    PostProcessVolume m_Volume;
    ChromaticAberration chromaticAberration;
    // Start is called before the first frame update
    void Start()
    {
        chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();



    }

    // Update is called once per frame
    void Update()
    {
        if (Lux.instance.luxMode)
        {
            chromaticAberration.enabled.Override(true);
            chromaticAberration.intensity.Override(0.5f);
            m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, chromaticAberration);
        }
        else
        {
            if (m_Volume != null)
            {
              
                chromaticAberration.enabled.Override(false);
                m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, chromaticAberration);
            }
        }

        
    }



}
