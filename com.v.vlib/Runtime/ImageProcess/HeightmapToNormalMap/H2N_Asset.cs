using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{


    [CreateAssetMenu()]
    [ExecuteInEditMode]
    public class H2N_Asset : ScriptableObject
    {
        public H2N_Setting setting = new H2N_Setting()
        {
            //Height Map Bluring Filter
            heightMapSetting = new HeightMap_BlurFilter()
            {
                preBlur = new GaussianBlurFilter()
                {
                    iteration = 0,
                    sampleFactor = 1
                },

                postBlur = new GaussianBlurFilter()
                {
                    iteration = 0,
                    sampleFactor = 1
                },

                factor = 1
            },

            normalMapSetting = new H2N_Generator()
            {
                sobeNormal_Filter = new SobelNormalFilter()
                {
                    bumpEffect = 0.5f
                },
                preBlur = new GaussianBlurFilter()
                {
                    iteration = 0,
                    sampleFactor = 1
                },
                postBlur = new GaussianBlurFilter()
                {
                    iteration = 1,
                    sampleFactor = 1
                }
            }

        };
    }
}
