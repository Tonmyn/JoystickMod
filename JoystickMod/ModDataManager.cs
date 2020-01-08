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

        public event Action<ModData> OnModDataChanged;

        private void Awake()
        {
            ModData = LoadData();
        }
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    var ja = new JoyAxis();
            //    ja.Name = "test joy axis";
            //    ja.Sensitivity = 5f;

            //    var list = ModData.joyAxes.ToList();
            //    list.Add(ja);
            //    ModData.joyAxes = list.ToArray(); ;
            //    SaveData();
            //}

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    LoadData();
            //    Debug.Log(ModData.ToString());
            //}

            //Debug.Log(Input.GetAxis("Mouse X"));
            //Debug.Log(Input.mousePosition.ToString());

        }

        public void AddAxis(JoyAxis joyAxis)
        {
            var axes = ModData.joyAxes.ToList();
            axes.Add(joyAxis);

            ModData.joyAxes = axes.ToArray();
            SaveData();
        }
        public void RemoveAxis(JoyAxis joyAxis)
        {
            var axes = ModData.joyAxes.ToList();

            if (axes.Exists(match => match.Guid == joyAxis.Guid))
            {
                var item = axes.Find(match => match.Guid == joyAxis.Guid);
                axes.Remove(item);
                ModData.joyAxes = axes.ToArray();
                SaveData();
            }
        }
        public void ReplaceAxis(JoyAxis joyAxis)
        {
            var axes = ModData.joyAxes.ToList();
            var axis = axes.Find(match => match.Guid == joyAxis.Guid);

            if (axis != null)
            {
                Debug.Log("replace");
                axes.RemoveAll(match => match.Guid == axis.Guid);
                axes.Add(joyAxis);
                ModData.joyAxes = axes.ToArray();

                SaveData();
            }
            else
            {
                AddAxis(joyAxis);
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
            OnModDataChanged?.Invoke(ModData);
            ModIO.SerializeXml(ModData ?? new ModData(), "Data.xml", true);
        }
    }
}
