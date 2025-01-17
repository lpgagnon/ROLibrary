﻿using System;
using UnityEngine;

namespace ROLib
{
    class ModelWindow : AbstractWindow
    {
        Vector2 selectModelScroll;
        private readonly ModuleROTank pm;
        public readonly ROLModelModule<ModuleROTank> module;
        private readonly ModelDefinitionLayoutOptions[] def;
        private string modelName;
        private string oldModel;

        public ModelWindow(ModuleROTank m, ROLModelModule<ModuleROTank> mod, ModelDefinitionLayoutOptions[] d, string name) :
            base(Guid.NewGuid(), $"ROTanks {name} Selection", new Rect(800, 350, 250, 600))
        {
            selectModelScroll = new Vector2();
            pm = m;
            module = mod;
            def = d;
        }

        private void UpdateModelSelections()
        {
            foreach (ModelDefinitionLayoutOptions options in def)
            {

                if (RenderToggleButton($"{options.definition.title}", options.definition.name == module.modelName))
                {
                    modelName = options.definition.name;
                    SelectCurrentModel(modelName);
                }
            }
        }

        private BaseField SetField(ModuleROTank mod)
        {
            BaseField theField;
            switch (module.name)
            {
                case "ModuleROTank-Mount":
                    theField = mod.Fields[nameof(mod.currentMount)];
                    oldModel = mod.currentMount;
                    break;
                case "ModuleROTank-Nose":
                    theField = mod.Fields[nameof(mod.currentNose)];
                    oldModel = mod.currentNose;
                    break;
                default:
                    theField = mod.Fields[nameof(mod.currentCore)];
                    oldModel = mod.currentCore;
                    break;
            }
            return theField;
        }

        private void SelectCurrentModel(string model)
        {
            BaseField fld = SetField(pm);
            fld.SetValue(model, pm);
            fld.uiControlEditor.onFieldChanged.Invoke(fld, oldModel);

            if (pm.part.symmetryCounterparts.Count > 0)
            {
                foreach (var p in pm.part.symmetryCounterparts)
                {
                    ModuleROTank m = (ModuleROTank)p.Modules["ModuleROTank"];
                    fld = SetField(m);
                    fld.SetValue(model, m);
                    fld.uiControlEditor.onFieldChanged.Invoke(fld, oldModel);
                }
            }
            //MonoUtilities.RefreshContextWindows(pm.part);
        }

        public void SelectModel()
        {
            selectModelScroll = GUILayout.BeginScrollView((selectModelScroll));
            UpdateModelSelections();
            GUILayout.EndScrollView();
        }

        public override void Window(int id)
        {
            using (new GUILayout.VerticalScope(GUILayout.Width(250), GUILayout.Height(500)))
            {
                SelectModel();
            }
            base.Window(id);
        }
    }
}
