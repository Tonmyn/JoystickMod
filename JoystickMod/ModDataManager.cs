using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;

namespace JoystickMod
{
    public class ModDataManager : MonoBehaviour
    {
        public ModData ModData { get; set; }

        private void Awake()
        {
            ModData = LoadData();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var ja = new JoyAxis();
                ja.Name = "test joy axis";
                ja.Sensitivity = 5f;

                var list = ModData.joyAxes.ToList();
                list.Add(ja);
                ModData.joyAxes = list.ToArray(); ;
                SaveData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadData();
                Debug.Log(ModData.ToString());
            }
        }

        public ModData LoadData()
        {
            ModData modData = new ModData();
            try
            {
                string filePath = "Data.xml";

                if (!ModIO.ExistsFile(filePath, true))
                {
                    SaveData();
                }
                else
                {
                    modData = ModIO.DeserializeXml<ModData>("Data.xml", true);
                }      
            }
            catch (Exception ex)
            {
                Debug.Log("Load ModData is error");
                Debug.Log(ex.Message);
            }
            return modData;
        }
        public void SaveData()
        {
            ModIO.SerializeXml(ModData ?? new ModData(), "Data.xml", true);
        }
    }
}
