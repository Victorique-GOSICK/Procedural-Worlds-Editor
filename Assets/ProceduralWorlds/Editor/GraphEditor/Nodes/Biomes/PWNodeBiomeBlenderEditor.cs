﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PW.Node;
using UnityEditor;
using PW.Biomator;

namespace PW.Editor
{
	[CustomEditor(typeof(PWNodeBiomeBlender))]
	public class PWNodeBiomeBlenderEditor : PWNodeEditor
	{
		public PWNodeBiomeBlender	node;

		string				updateBiomeMapKey = "BiomeBlender";

		public override void OnNodeEnable()
		{
			node = target as PWNodeBiomeBlender;
			
			delayedChanges.BindCallback(updateBiomeMapKey, (unused) => {
				BiomeData data = node.GetBiomeData();

				node.FillBiomeMap(data);
				node.updateBiomeMap = true;

				NotifyReload();
			});
			
		}

		public override void OnNodeGUI()
		{
			BiomeData biomeData = node.GetBiomeData();

			if (biomeData == null)
			{
				EditorGUILayout.LabelField("biomes not connected !");
				return ;
			}
			else
			{
				EditorGUIUtility.labelWidth = 120;
				EditorGUI.BeginChangeCheck();
				biomeBlendPercent = PWGUI.Slider("Biome blend ratio: ", biomeBlendPercent, 0f, .5f);
				if (EditorGUI.EndChangeCheck())
					delayedChanges.UpdateValue(updateBiomeMapKey);
				blendList.UpdateIfNeeded(biomeData);

				EditorGUI.BeginChangeCheck();
				blendList.DrawList(biomeData, visualRect);
				if (EditorGUI.EndChangeCheck())
					delayedChanges.UpdateValue(updateBiomeMapKey);
			}

			if (biomeData != null)
			{
				if (biomeData.biomeMap != null)
					PWGUI.BiomeMap2DPreview(biomeData);
				//TODO: biome 3D preview
			}
			else
				EditorGUILayout.LabelField("no biome data");
			
			if (updateBiomeMap)
			{
				PWGUI.SetUpdateForField(1, true);
				updateBiomeMap = false;
			}

			var biomeCoverage = biomeData.biomeSwitchGraph.GetBiomeCoverage();

			bool biomeCoverageError = biomeCoverage.Any(b => b.Value > 0 && b.Value < 1);

			GUIStyle biomeCoverageFoloutStyle = (biomeCoverageError) ? PWStyles.errorFoldout : EditorStyles.foldout;

			if (biomeCoverageRecap = EditorGUILayout.Foldout(biomeCoverageRecap, "Biome coverage recap", biomeCoverageFoloutStyle))
			{
				if (biomeData != null && biomeData.biomeSwitchGraph != null)
				{
					foreach (var biomeCoverageKP in biomeCoverage)
						if (biomeCoverageKP.Value > 0)
						{
							string paramName = biomeData.GetBiomeKey(biomeCoverageKP.Key);
							EditorGUILayout.LabelField(paramName, (biomeCoverageKP.Value * 100).ToString("F2") + "%");
						}
				}
				else
					EditorGUILayout.LabelField("Null biome data/biome tree");
			}
		}

		public override void OnNodeDisable()
		{
			
		}
	}
}