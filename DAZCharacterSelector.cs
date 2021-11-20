using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DAZCharacterSelector : JSONStorable
{
	public enum Gender
	{
		None,
		Male,
		Female,
		Both
	}

	public DAZBones rootBones;

	public DAZMorphBank femaleMorphBank1;

	public DAZMorphBank femaleMorphBank2;

	public DAZMorphBank maleMorphBank1;

	public DAZMorphBank maleMorphBank2;

	[HideInInspector]
	[SerializeField]
	protected Gender _gender;

	public Transform maleCharactersContainer;

	public Transform femaleCharactersContainer;

	public Transform maleClothingContainer;

	public Transform femaleClothingContainer;

	public Transform maleHairContainer;

	public Transform femaleHairContainer;

	public Transform[] maleTransforms;

	public Transform[] femaleTransforms;

	public string rootBonesName = "Genesis2";

	public string rootBonesNameMale = "Genesis2Male";

	public string rootBonesNameFemale = "Genesis2Female";

	public Transform[] regularColliders;

	public Transform[] regularCollidersFemale;

	public Transform[] regularCollidersMale;

	public Transform[] advancedCollidersFemale;

	public Transform[] advancedCollidersMale;

	public Toggle useAdvancedCollidersToggle;

	[SerializeField]
	protected bool _useAdvancedColliders;

	public int maleAnatomyMaterialSlot = 29;

	private DAZMaleAnatomy[] maleAnatomyComponents;

	public DAZCharacter startingCharacter;

	private Dictionary<string, DAZCharacter> _characterByName;

	private DAZCharacter[] _femaleCharacters;

	private DAZCharacter[] _maleCharacters;

	private DAZCharacter[] _characters;

	private DAZMorphSet activeMorphSet;

	private DAZCharacter _selectedCharacter;

	protected AsyncFlag onCharacterLoadedFlag;

	public bool[] startingActiveClothingItems;

	private bool[] _activeClothingItems;

	private bool[] _clothingItemsDisableMaleAnatomy;

	private Dictionary<string, DAZClothingItem> _clothingItemByName;

	private Dictionary<string, int> _clothingItemNameToIndex;

	private DAZClothingItem[] _maleClothingItems;

	private DAZClothingItem[] _femaleClothingItems;

	private DAZHairGroup[] _maleHairGroups;

	private DAZHairGroup[] _femaleHairGroups;

	public DAZHairGroup startingHairGroup;

	private DAZHairGroup _selectedHairGroup;

	public GenerateDAZCharacterSelectorUI characterSelectorUI;

	public GenerateDAZCharacterSelectorUI characterSelectorUIAlt;

	public GenerateDAZMorphsControlUI morphsControlUI;

	public GenerateDAZMorphsControlUI morphsControlUIAlt;

	public GenerateDAZClothingSelectorUI clothingSelectorUI;

	public GenerateDAZClothingSelectorUI clothingSelectorUIAlt;

	public GenerateDAZHairSelectorUI hairSelectorUI;

	public GenerateDAZHairSelectorUI hairSelectorUIAlt;

	public DAZCharacterMaterialOptions copyUIFrom;

	public Text color1DisplayNameText;

	public HSVColorPicker color1Picker;

	public RectTransform color1Container;

	public Text color2DisplayNameText;

	public HSVColorPicker color2Picker;

	public RectTransform color2Container;

	public Text color3DisplayNameText;

	public HSVColorPicker color3Picker;

	public RectTransform color3Container;

	public Text param1DisplayNameText;

	public Slider param1Slider;

	public Text param1DisplayNameTextAlt;

	public Slider param1SliderAlt;

	public Text param2DisplayNameText;

	public Slider param2Slider;

	public Text param2DisplayNameTextAlt;

	public Slider param2SliderAlt;

	public Text param3DisplayNameText;

	public Slider param3Slider;

	public Text param3DisplayNameTextAlt;

	public Slider param3SliderAlt;

	public Text param4DisplayNameText;

	public Slider param4Slider;

	public Text param4DisplayNameTextAlt;

	public Slider param4SliderAlt;

	public Text param5DisplayNameText;

	public Slider param5Slider;

	public Text param5DisplayNameTextAlt;

	public Slider param5SliderAlt;

	public Text param6DisplayNameText;

	public Slider param6Slider;

	public Text param6DisplayNameTextAlt;

	public Slider param6SliderAlt;

	public Text param7DisplayNameText;

	public Slider param7Slider;

	public Text param7DisplayNameTextAlt;

	public Slider param7SliderAlt;

	public Text param8DisplayNameText;

	public Slider param8Slider;

	public Text param8DisplayNameTextAlt;

	public Slider param8SliderAlt;

	public UIPopup textureGroup1Popup;

	public Text textureGroup1Text;

	public UIPopup textureGroup1PopupAlt;

	public Text textureGroup1TextAlt;

	public UIPopup textureGroup2Popup;

	public Text textureGroup2Text;

	public UIPopup textureGroup2PopupAlt;

	public Text textureGroup2TextAlt;

	public UIPopup textureGroup3Popup;

	public Text textureGroup3Text;

	public UIPopup textureGroup3PopupAlt;

	public Text textureGroup3TextAlt;

	public UIPopup textureGroup4Popup;

	public Text textureGroup4Text;

	public UIPopup textureGroup4PopupAlt;

	public Text textureGroup4TextAlt;

	public UIPopup textureGroup5Popup;

	public Text textureGroup5Text;

	public UIPopup textureGroup5PopupAlt;

	public Text textureGroup5TextAlt;

	private AutoColliderBatchUpdater[] _autoColliderBatchUpdaters;

	private AutoCollider[] _autoColliders;

	private DAZPhysicsMesh[] _physicsMeshes;

	private SetAnchorFromVertex[] _setAnchorFromVertexComps;

	private DAZCharacterMaterialOptions[] _materialOptions;

	private IgnoreChildColliders[] _ignoreChildColliders;

	private bool wasInit;

	public DAZMorphBank morphBank1 => gender switch
	{
		Gender.Female => femaleMorphBank1, 
		Gender.Male => maleMorphBank1, 
		_ => null, 
	};

	public DAZMorphBank morphBank2 => gender switch
	{
		Gender.Female => femaleMorphBank2, 
		Gender.Male => maleMorphBank2, 
		_ => null, 
	};

	public Gender gender
	{
		get
		{
			return _gender;
		}
		set
		{
			if (_gender != value)
			{
				_gender = value;
				SyncGender();
			}
		}
	}

	public bool useAdvancedColliders
	{
		get
		{
			return _useAdvancedColliders;
		}
		set
		{
			if (_useAdvancedColliders != value)
			{
				_useAdvancedColliders = value;
				if (useAdvancedCollidersToggle != null)
				{
					useAdvancedCollidersToggle.isOn = _useAdvancedColliders;
				}
				SyncColliders();
			}
		}
	}

	public DAZCharacter[] femaleCharacters => _femaleCharacters;

	public DAZCharacter[] maleCharacters => _maleCharacters;

	public DAZCharacter[] characters => _characters;

	public DAZCharacter selectedCharacter
	{
		get
		{
			return _selectedCharacter;
		}
		set
		{
			if (!(_selectedCharacter != value))
			{
				return;
			}
			if (_selectedCharacter != null)
			{
				if (activeMorphSet != null)
				{
					activeMorphSet.DAZMeshInitTransform = _selectedCharacter.transform;
					activeMorphSet.InitSet();
				}
				_selectedCharacter.gameObject.SetActive(value: false);
			}
			if (Application.isPlaying)
			{
				if (_physicsMeshes != null)
				{
					DAZPhysicsMesh[] physicsMeshes = _physicsMeshes;
					foreach (DAZPhysicsMesh dAZPhysicsMesh in physicsMeshes)
					{
						if (dAZPhysicsMesh != null)
						{
							dAZPhysicsMesh.PauseSimulation();
						}
					}
				}
				if (_autoColliderBatchUpdaters != null)
				{
					AutoColliderBatchUpdater[] autoColliderBatchUpdaters = _autoColliderBatchUpdaters;
					foreach (AutoColliderBatchUpdater autoColliderBatchUpdater in autoColliderBatchUpdaters)
					{
						if (autoColliderBatchUpdater != null)
						{
							autoColliderBatchUpdater.PauseSimulation();
						}
					}
				}
				if (_autoColliders != null)
				{
					AutoCollider[] autoColliders = _autoColliders;
					foreach (AutoCollider autoCollider in autoColliders)
					{
						if (autoCollider != null && !autoCollider.allowBatchUpdate)
						{
							autoCollider.PauseSimulation();
						}
					}
				}
				if (_setAnchorFromVertexComps != null)
				{
					SetAnchorFromVertex[] setAnchorFromVertexComps = _setAnchorFromVertexComps;
					foreach (SetAnchorFromVertex setAnchorFromVertex in setAnchorFromVertexComps)
					{
						if (setAnchorFromVertex != null)
						{
							setAnchorFromVertex.PauseSimulation();
						}
					}
				}
				for (int m = 0; m < clothingItems.Length; m++)
				{
					DAZClothingItem dAZClothingItem = clothingItems[m];
					if (_activeClothingItems[m])
					{
						AutoCollider[] componentsInChildren = dAZClothingItem.GetComponentsInChildren<AutoCollider>();
						AutoCollider[] array = componentsInChildren;
						foreach (AutoCollider autoCollider2 in array)
						{
							autoCollider2.PauseSimulation();
						}
					}
				}
			}
			_selectedCharacter = value;
			if (!(_selectedCharacter != null))
			{
				return;
			}
			if (_selectedCharacter.isMale)
			{
				if (_gender != Gender.Male)
				{
					gender = Gender.Male;
				}
			}
			else if (_gender != Gender.Female)
			{
				gender = Gender.Female;
			}
			if (characterSelectorUI != null)
			{
				characterSelectorUI.SetActiveCharacterToggle(_selectedCharacter.displayName);
			}
			if (characterSelectorUIAlt != null)
			{
				characterSelectorUIAlt.SetActiveCharacterToggle(_selectedCharacter.displayName);
			}
			if (Application.isPlaying)
			{
				if (onCharacterLoadedFlag != null && !onCharacterLoadedFlag.flag)
				{
					Debug.LogWarning("onCharacterLoadedFlag still set while trying to load another. Set to " + onCharacterLoadedFlag.name);
					onCharacterLoadedFlag.flag = true;
				}
				onCharacterLoadedFlag = new AsyncFlag();
				onCharacterLoadedFlag.name = _selectedCharacter.displayName;
				if (_physicsMeshes != null)
				{
					DAZPhysicsMesh[] physicsMeshes2 = _physicsMeshes;
					foreach (DAZPhysicsMesh dAZPhysicsMesh2 in physicsMeshes2)
					{
						if (dAZPhysicsMesh2 != null)
						{
							dAZPhysicsMesh2.PauseSimulation(onCharacterLoadedFlag);
						}
					}
				}
				if (_autoColliderBatchUpdaters != null)
				{
					AutoColliderBatchUpdater[] autoColliderBatchUpdaters2 = _autoColliderBatchUpdaters;
					foreach (AutoColliderBatchUpdater autoColliderBatchUpdater2 in autoColliderBatchUpdaters2)
					{
						if (autoColliderBatchUpdater2 != null)
						{
							autoColliderBatchUpdater2.PauseSimulation(onCharacterLoadedFlag);
						}
					}
				}
				if (_autoColliders != null)
				{
					AutoCollider[] autoColliders2 = _autoColliders;
					foreach (AutoCollider autoCollider3 in autoColliders2)
					{
						if (autoCollider3 != null && !autoCollider3.allowBatchUpdate)
						{
							autoCollider3.PauseSimulation(onCharacterLoadedFlag);
						}
					}
				}
				if (_setAnchorFromVertexComps != null)
				{
					SetAnchorFromVertex[] setAnchorFromVertexComps2 = _setAnchorFromVertexComps;
					foreach (SetAnchorFromVertex setAnchorFromVertex2 in setAnchorFromVertexComps2)
					{
						if (setAnchorFromVertex2 != null)
						{
							setAnchorFromVertex2.PauseSimulation(onCharacterLoadedFlag);
						}
					}
				}
				for (int num5 = 0; num5 < clothingItems.Length; num5++)
				{
					DAZClothingItem dAZClothingItem2 = clothingItems[num5];
					if (_activeClothingItems[num5])
					{
						AutoCollider[] componentsInChildren2 = dAZClothingItem2.GetComponentsInChildren<AutoCollider>();
						AutoCollider[] array2 = componentsInChildren2;
						foreach (AutoCollider autoCollider4 in array2)
						{
							autoCollider4.PauseSimulation(onCharacterLoadedFlag);
						}
					}
				}
			}
			DAZCharacter dAZCharacter = _selectedCharacter;
			dAZCharacter.onLoadedHandlers = (DAZCharacter.OnLoaded)Delegate.Combine(dAZCharacter.onLoadedHandlers, new DAZCharacter.OnLoaded(OnCharacterLoaded));
			_selectedCharacter.gameObject.SetActive(value: true);
		}
	}

	public bool[] activeClothingItems
	{
		get
		{
			Init();
			return _activeClothingItems;
		}
	}

	public DAZClothingItem[] maleClothingItems
	{
		get
		{
			Init();
			return _maleClothingItems;
		}
	}

	public DAZClothingItem[] femaleClothingItems
	{
		get
		{
			Init();
			return _femaleClothingItems;
		}
	}

	public DAZClothingItem[] clothingItems
	{
		get
		{
			Init();
			if (gender == Gender.Male)
			{
				return _maleClothingItems;
			}
			if (gender == Gender.Female)
			{
				return _femaleClothingItems;
			}
			return null;
		}
	}

	public DAZHairGroup[] hairGroups
	{
		get
		{
			Init();
			if (gender == Gender.Male)
			{
				return _maleHairGroups;
			}
			if (gender == Gender.Female)
			{
				return _femaleHairGroups;
			}
			return null;
		}
	}

	public DAZHairGroup selectedHairGroup
	{
		get
		{
			Init();
			return _selectedHairGroup;
		}
		set
		{
			if (_selectedHairGroup != null)
			{
				_selectedHairGroup.gameObject.SetActive(value: false);
			}
			_selectedHairGroup = value;
			if (_selectedHairGroup != null)
			{
				if (hairSelectorUI != null)
				{
					hairSelectorUI.SetActiveHairToggle(_selectedHairGroup.displayName);
				}
				if (hairSelectorUIAlt != null)
				{
					hairSelectorUIAlt.SetActiveHairToggle(_selectedHairGroup.displayName);
				}
				_selectedHairGroup.gameObject.SetActive(value: true);
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		JSONArray jSONArray = new JSONArray();
		if (includeAppearance)
		{
			needsStore = true;
			jSON["character"] = selectedCharacter.displayName;
			jSON["clothing"] = jSONArray;
			for (int i = 0; i < clothingItems.Length; i++)
			{
				DAZClothingItem dAZClothingItem = clothingItems[i];
				if (_activeClothingItems[i])
				{
					JSONClass jSONClass = new JSONClass();
					jSONArray.Add(jSONClass);
					jSONClass["name"] = dAZClothingItem.displayName;
					jSONClass["enabled"].AsBool = _activeClothingItems[i];
				}
			}
			if (selectedHairGroup != null)
			{
				jSON["hair"] = selectedHairGroup.displayName;
			}
		}
		jSONArray = (JSONArray)(jSON["morphs"] = new JSONArray());
		if (morphsControlUI != null)
		{
			List<string> morphDisplayNames = morphsControlUI.GetMorphDisplayNames();
			if (morphDisplayNames != null)
			{
				{
					foreach (string item in morphDisplayNames)
					{
						bool flag = morphsControlUI.IsMorphPoseControl(item);
						if ((includePhysical && flag) || (includeAppearance && !flag))
						{
							float morphValue = morphsControlUI.GetMorphValue(item);
							float morphDefaultValue = morphsControlUI.GetMorphDefaultValue(item);
							if (morphValue != morphDefaultValue)
							{
								needsStore = true;
								JSONClass jSONClass2 = new JSONClass();
								jSONClass2["name"] = item;
								jSONClass2["value"].AsFloat = morphValue;
								jSONArray.Add(jSONClass2);
							}
						}
					}
					return jSON;
				}
			}
			Debug.LogWarning("morphDisplayNames not set for " + base.name + " character " + selectedCharacter.displayName);
		}
		else
		{
			Debug.LogWarning("morphsControl UI not set for " + base.name + " character " + selectedCharacter.displayName);
		}
		return jSON;
	}

	private void ResetClothing(bool clearAll = false)
	{
		Init();
		for (int i = 0; i < clothingItems.Length; i++)
		{
			if (clearAll)
			{
				SetActiveClothingItem(i, active: false);
			}
			else
			{
				SetActiveClothingItem(i, startingActiveClothingItems[i]);
			}
		}
	}

	public void ResetMorphs(bool resetPhysical, bool resetAppearance)
	{
		Init();
		if (morphsControlUI != null)
		{
			List<string> morphDisplayNames = morphsControlUI.GetMorphDisplayNames();
			if (morphDisplayNames != null)
			{
				foreach (string item in morphDisplayNames)
				{
					bool flag = morphsControlUI.IsMorphPoseControl(item);
					if ((resetPhysical && flag) || (resetAppearance && !flag))
					{
						float morphDefaultValue = morphsControlUI.GetMorphDefaultValue(item);
						morphsControlUI.SetMorphValue(item, morphDefaultValue);
						if (morphsControlUIAlt != null)
						{
							morphsControlUIAlt.SetMorphValue(item, morphDefaultValue);
						}
					}
				}
			}
		}
		morphsControlUI.ResetMorphs();
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		Init();
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (restoreAppearance)
		{
			if (jc["character"] != null)
			{
				string characterName = jc["character"];
				selectCharacterByName(characterName);
			}
			else
			{
				selectedCharacter = startingCharacter;
			}
			if (jc["hair"] != null)
			{
				string text = jc["hair"];
				SetSelectedHairGroup(text);
			}
			else
			{
				selectedHairGroup = startingHairGroup;
			}
			ResetClothing();
			if (jc["clothing"] != null)
			{
				ResetClothing(clearAll: true);
				foreach (JSONClass item in jc["clothing"].AsArray)
				{
					string itemName = item["name"];
					bool asBool = item["enabled"].AsBool;
					SetActiveClothingItem(itemName, asBool);
				}
			}
		}
		ResetMorphs(restorePhysical, restoreAppearance);
		if (jc["morphs"] != null && morphsControlUI != null)
		{
			foreach (JSONClass item2 in jc["morphs"].AsArray)
			{
				string text2 = item2["name"];
				float asFloat = item2["value"].AsFloat;
				if (text2 == null)
				{
					continue;
				}
				bool flag = morphsControlUI.IsMorphPoseControl(text2);
				if ((restorePhysical && flag) || (restoreAppearance && !flag))
				{
					morphsControlUI.SetMorphValue(text2, asFloat);
					if ((bool)morphsControlUIAlt)
					{
						morphsControlUIAlt.SetMorphValue(text2, asFloat);
					}
				}
			}
		}
		if (morphBank1 != null)
		{
			morphBank1.ApplyMorphsImmediate();
		}
		if (morphBank2 != null)
		{
			morphBank2.ApplyMorphsImmediate();
		}
		PauseSimulation10Frames();
	}

	protected void SyncGender()
	{
		Transform[] array = maleTransforms;
		foreach (Transform transform in array)
		{
			if (_gender == Gender.Both || _gender == Gender.Male)
			{
				transform.gameObject.SetActive(value: true);
			}
			else
			{
				transform.gameObject.SetActive(value: false);
			}
		}
		Transform[] array2 = femaleTransforms;
		foreach (Transform transform2 in array2)
		{
			if (_gender == Gender.Both || _gender == Gender.Female)
			{
				transform2.gameObject.SetActive(value: true);
			}
			else
			{
				transform2.gameObject.SetActive(value: false);
			}
		}
		if (rootBones != null)
		{
			if (_gender == Gender.Male)
			{
				rootBones.name = rootBonesNameMale;
				rootBones.isMale = true;
			}
			else if (_gender == Gender.Female)
			{
				rootBones.name = rootBonesNameFemale;
				rootBones.isMale = false;
			}
			else
			{
				rootBones.name = rootBonesName;
				rootBones.isMale = false;
			}
		}
		Init(force: true);
		SyncColliders();
	}

	protected void SyncColliders()
	{
		Transform[] array = regularCollidersFemale;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(!_useAdvancedColliders && _gender == Gender.Female);
		}
		Transform[] array2 = advancedCollidersFemale;
		foreach (Transform transform2 in array2)
		{
			transform2.gameObject.SetActive(_useAdvancedColliders && _gender == Gender.Female);
		}
		Transform[] array3 = regularCollidersMale;
		foreach (Transform transform3 in array3)
		{
			transform3.gameObject.SetActive(!_useAdvancedColliders && _gender == Gender.Male);
		}
		Transform[] array4 = advancedCollidersMale;
		foreach (Transform transform4 in array4)
		{
			transform4.gameObject.SetActive(_useAdvancedColliders && _gender == Gender.Male);
		}
		Transform[] array5 = regularColliders;
		foreach (Transform transform5 in array5)
		{
			transform5.gameObject.SetActive(!_useAdvancedColliders);
		}
		if (!Application.isPlaying)
		{
			return;
		}
		IgnoreChildColliders[] ignoreChildColliders = _ignoreChildColliders;
		foreach (IgnoreChildColliders ignoreChildColliders2 in ignoreChildColliders)
		{
			ignoreChildColliders2.SyncColliders();
		}
		DAZPhysicsMesh[] physicsMeshes = _physicsMeshes;
		foreach (DAZPhysicsMesh dAZPhysicsMesh in physicsMeshes)
		{
			dAZPhysicsMesh.InitColliders();
		}
		AutoColliderBatchUpdater[] autoColliderBatchUpdaters = _autoColliderBatchUpdaters;
		foreach (AutoColliderBatchUpdater autoColliderBatchUpdater in autoColliderBatchUpdaters)
		{
			if (autoColliderBatchUpdater != null)
			{
				autoColliderBatchUpdater.UpdateAutoColliders();
			}
		}
	}

	public void InitBones()
	{
		if (rootBones != null)
		{
			rootBones.Init();
			maleAnatomyComponents = rootBones.GetComponentsInChildren<DAZMaleAnatomy>(includeInactive: true);
		}
	}

	public void InitCharacters()
	{
		if (femaleCharactersContainer != null)
		{
			_femaleCharacters = femaleCharactersContainer.GetComponentsInChildren<DAZCharacter>(includeInactive: true);
		}
		else
		{
			_femaleCharacters = new DAZCharacter[0];
		}
		if (maleCharactersContainer != null)
		{
			_maleCharacters = maleCharactersContainer.GetComponentsInChildren<DAZCharacter>(includeInactive: true);
		}
		else
		{
			_maleCharacters = new DAZCharacter[0];
		}
		_characterByName = new Dictionary<string, DAZCharacter>();
		_characters = new DAZCharacter[_femaleCharacters.Length + _maleCharacters.Length];
		int num = 0;
		for (int i = 0; i < _femaleCharacters.Length; i++)
		{
			_characters[num] = _femaleCharacters[i];
			num++;
		}
		for (int j = 0; j < _maleCharacters.Length; j++)
		{
			_characters[num] = _maleCharacters[j];
			num++;
		}
		DAZCharacter[] array = _characters;
		foreach (DAZCharacter dAZCharacter in array)
		{
			if (!(dAZCharacter != null))
			{
				continue;
			}
			if (_characterByName.ContainsKey(dAZCharacter.displayName))
			{
				Debug.LogError("Character " + dAZCharacter.displayName + " is a duplicate. Cannot add");
				continue;
			}
			_characterByName.Add(dAZCharacter.displayName, dAZCharacter);
			if (dAZCharacter.gameObject.activeSelf)
			{
				_selectedCharacter = dAZCharacter;
			}
		}
	}

	public void selectCharacterByName(string characterName)
	{
		if (_characterByName == null)
		{
			Init();
		}
		if (_characterByName.TryGetValue(characterName, out var value))
		{
			selectedCharacter = value;
		}
	}

	protected void ConnectSkin()
	{
		DAZSkinV2 skin = _selectedCharacter.skin;
		if (skin != null)
		{
			if (_physicsMeshes != null)
			{
				DAZPhysicsMesh[] physicsMeshes = _physicsMeshes;
				foreach (DAZPhysicsMesh dAZPhysicsMesh in physicsMeshes)
				{
					if (dAZPhysicsMesh != null)
					{
						dAZPhysicsMesh.skinTransform = skin.transform;
						dAZPhysicsMesh.skin = skin;
					}
				}
			}
			if (_autoColliderBatchUpdaters != null)
			{
				AutoColliderBatchUpdater[] autoColliderBatchUpdaters = _autoColliderBatchUpdaters;
				foreach (AutoColliderBatchUpdater autoColliderBatchUpdater in autoColliderBatchUpdaters)
				{
					autoColliderBatchUpdater.skin = skin;
				}
			}
			if (_autoColliders != null)
			{
				AutoCollider[] autoColliders = _autoColliders;
				foreach (AutoCollider autoCollider in autoColliders)
				{
					if (autoCollider != null)
					{
						autoCollider.skinTransform = skin.transform;
						autoCollider.skin = skin;
						if (morphBank1 != null)
						{
							autoCollider.morphBank1ForResizeTrigger = morphBank1;
						}
						if (morphBank2 != null)
						{
							autoCollider.morphBank2ForResizeTrigger = morphBank2;
						}
					}
				}
			}
			if (_setAnchorFromVertexComps != null)
			{
				SetAnchorFromVertex[] setAnchorFromVertexComps = _setAnchorFromVertexComps;
				foreach (SetAnchorFromVertex setAnchorFromVertex in setAnchorFromVertexComps)
				{
					if (setAnchorFromVertex != null)
					{
						setAnchorFromVertex.skinTransform = skin.transform;
						setAnchorFromVertex.skin = skin;
					}
				}
			}
			DAZCharacterMaterialOptions[] materialOptions = _materialOptions;
			foreach (DAZCharacterMaterialOptions dAZCharacterMaterialOptions in materialOptions)
			{
				if (dAZCharacterMaterialOptions != null)
				{
					dAZCharacterMaterialOptions.skin = skin;
				}
			}
			ConnectCharacterMaterialOptionsUI();
			DAZClothingItem[] array = clothingItems;
			foreach (DAZClothingItem dAZClothingItem in array)
			{
				if (dAZClothingItem != null)
				{
					dAZClothingItem.skin = skin;
				}
			}
			DAZHairGroup[] array2 = hairGroups;
			foreach (DAZHairGroup dAZHairGroup in array2)
			{
				if (dAZHairGroup != null)
				{
					dAZHairGroup.skin = skin;
				}
			}
		}
		else
		{
			Debug.LogWarning("No skin found during ConnectSkin for " + _selectedCharacter.displayName);
		}
	}

	protected void PauseSimulation10Frames()
	{
		if (_physicsMeshes != null)
		{
			DAZPhysicsMesh[] physicsMeshes = _physicsMeshes;
			foreach (DAZPhysicsMesh dAZPhysicsMesh in physicsMeshes)
			{
				if (dAZPhysicsMesh != null)
				{
					dAZPhysicsMesh.PauseSimulation(10);
				}
			}
		}
		if (_autoColliderBatchUpdaters != null)
		{
			AutoColliderBatchUpdater[] autoColliderBatchUpdaters = _autoColliderBatchUpdaters;
			foreach (AutoColliderBatchUpdater autoColliderBatchUpdater in autoColliderBatchUpdaters)
			{
				if (autoColliderBatchUpdater != null)
				{
					autoColliderBatchUpdater.PauseSimulation(10);
				}
			}
		}
		if (_autoColliders != null)
		{
			AutoCollider[] autoColliders = _autoColliders;
			foreach (AutoCollider autoCollider in autoColliders)
			{
				if (autoCollider != null && !autoCollider.allowBatchUpdate)
				{
					autoCollider.PauseSimulation(10);
				}
			}
		}
		if (_setAnchorFromVertexComps != null)
		{
			SetAnchorFromVertex[] setAnchorFromVertexComps = _setAnchorFromVertexComps;
			foreach (SetAnchorFromVertex setAnchorFromVertex in setAnchorFromVertexComps)
			{
				if (setAnchorFromVertex != null)
				{
					setAnchorFromVertex.PauseSimulation(10);
				}
			}
		}
		for (int m = 0; m < clothingItems.Length; m++)
		{
			DAZClothingItem dAZClothingItem = clothingItems[m];
			if (_activeClothingItems[m])
			{
				AutoCollider[] componentsInChildren = dAZClothingItem.GetComponentsInChildren<AutoCollider>();
				AutoCollider[] array = componentsInChildren;
				foreach (AutoCollider autoCollider2 in array)
				{
					autoCollider2.PauseSimulation(10);
				}
			}
		}
	}

	protected void OnCharacterLoaded()
	{
		DAZMesh[] componentsInChildren = _selectedCharacter.GetComponentsInChildren<DAZMesh>(includeInactive: true);
		DAZMesh[] array = componentsInChildren;
		foreach (DAZMesh dAZMesh in array)
		{
			if (morphBank1 != null && morphBank1.geometryId == dAZMesh.geometryId)
			{
				morphBank1.connectedMesh = dAZMesh;
				dAZMesh.morphBank = morphBank1;
			}
			if (morphBank2 != null && morphBank2.geometryId == dAZMesh.geometryId)
			{
				morphBank2.connectedMesh = dAZMesh;
				dAZMesh.morphBank = morphBank2;
			}
		}
		ConnectSkin();
		if (activeMorphSet != null)
		{
			activeMorphSet.DAZMeshApplyTransform = _selectedCharacter.transform;
			activeMorphSet.ApplySet();
		}
		SyncMaleAnatomy();
		if (onCharacterLoadedFlag != null)
		{
			onCharacterLoadedFlag.flag = true;
		}
		PauseSimulation10Frames();
	}

	public void InitClothingItems()
	{
		if (maleClothingContainer != null)
		{
			_maleClothingItems = maleClothingContainer.GetComponentsInChildren<DAZClothingItem>(includeInactive: true);
		}
		else
		{
			_maleClothingItems = new DAZClothingItem[0];
		}
		if (femaleClothingContainer != null)
		{
			_femaleClothingItems = femaleClothingContainer.GetComponentsInChildren<DAZClothingItem>(includeInactive: true);
		}
		else
		{
			_femaleClothingItems = new DAZClothingItem[0];
		}
		_clothingItemByName = new Dictionary<string, DAZClothingItem>();
		_clothingItemNameToIndex = new Dictionary<string, int>();
		_activeClothingItems = new bool[clothingItems.Length];
		_clothingItemsDisableMaleAnatomy = new bool[clothingItems.Length];
		int num = 0;
		DAZClothingItem[] array = clothingItems;
		foreach (DAZClothingItem dAZClothingItem in array)
		{
			_clothingItemByName.Add(dAZClothingItem.displayName, dAZClothingItem);
			_clothingItemNameToIndex.Add(dAZClothingItem.displayName, num);
			if (dAZClothingItem.gameObject.activeSelf)
			{
				_activeClothingItems[num] = true;
			}
			_clothingItemsDisableMaleAnatomy[num] = dAZClothingItem.disableMaleAnatomy;
			num++;
		}
	}

	private void SyncMaleAnatomy()
	{
		if (gender != Gender.Male || !(_selectedCharacter != null))
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < _activeClothingItems.Length; i++)
		{
			if (_activeClothingItems[i] && _clothingItemsDisableMaleAnatomy[i])
			{
				flag = false;
				break;
			}
		}
		if (maleAnatomyComponents != null && maleAnatomyComponents.Length > 0)
		{
			DAZMaleAnatomy[] array = maleAnatomyComponents;
			foreach (DAZMaleAnatomy dAZMaleAnatomy in array)
			{
				Rigidbody[] componentsInChildren = dAZMaleAnatomy.GetComponentsInChildren<Rigidbody>();
				Rigidbody[] array2 = componentsInChildren;
				foreach (Rigidbody rigidbody in array2)
				{
					rigidbody.detectCollisions = flag;
				}
			}
		}
		DAZSkinV2 skin = _selectedCharacter.skin;
		if (skin != null && skin.materialsEnabled.Length > maleAnatomyMaterialSlot)
		{
			skin.materialsEnabled[maleAnatomyMaterialSlot] = flag;
		}
		if (skin != null && skin.dazMesh != null && skin.dazMesh.materialsEnabled.Length > maleAnatomyMaterialSlot)
		{
			skin.dazMesh.materialsEnabled[maleAnatomyMaterialSlot] = flag;
		}
	}

	public int GetClothingItemIndex(string itemName)
	{
		if (_clothingItemNameToIndex == null)
		{
			Init();
		}
		if (_clothingItemNameToIndex.TryGetValue(itemName, out var value))
		{
			return value;
		}
		return -1;
	}

	public void ToggleClothingItemByName(string itemName)
	{
		int clothingItemIndex = GetClothingItemIndex(itemName);
		if (clothingItemIndex >= 0 && clothingItemIndex <= _activeClothingItems.Length)
		{
			SetActiveClothingItem(clothingItemIndex, !_activeClothingItems[clothingItemIndex]);
		}
	}

	public bool IsClothingItemActive(int index)
	{
		if (index >= 0 && index <= _activeClothingItems.Length)
		{
			return _activeClothingItems[index];
		}
		return false;
	}

	public bool IsClothingItemActive(string itemName)
	{
		return IsClothingItemActive(GetClothingItemIndex(itemName));
	}

	public void SetActiveClothingItem(int index, bool active)
	{
		if (index >= 0 && index <= _activeClothingItems.Length)
		{
			_activeClothingItems[index] = active;
			clothingItems[index].gameObject.SetActive(active);
			if (clothingSelectorUI != null)
			{
				clothingSelectorUI.SetClothingItemToggle(clothingItems[index].displayName, active);
			}
			if (clothingSelectorUIAlt != null)
			{
				clothingSelectorUIAlt.SetClothingItemToggle(clothingItems[index].displayName, active);
			}
			SyncMaleAnatomy();
		}
	}

	public void SetActiveClothingItem(string itemName, bool active)
	{
		int clothingItemIndex = GetClothingItemIndex(itemName);
		SetActiveClothingItem(clothingItemIndex, active);
	}

	public void InitHair()
	{
		if (maleHairContainer != null)
		{
			_maleHairGroups = maleHairContainer.GetComponentsInChildren<DAZHairGroup>(includeInactive: true);
		}
		else
		{
			_maleHairGroups = new DAZHairGroup[0];
		}
		if (femaleHairContainer != null)
		{
			_femaleHairGroups = femaleHairContainer.GetComponentsInChildren<DAZHairGroup>(includeInactive: true);
		}
		else
		{
			_femaleHairGroups = new DAZHairGroup[0];
		}
		DAZHairGroup[] array = hairGroups;
		foreach (DAZHairGroup dAZHairGroup in array)
		{
			if (dAZHairGroup.gameObject.activeSelf)
			{
				selectedHairGroup = dAZHairGroup;
			}
		}
	}

	public void SetSelectedHairGroup(string hairGroupName)
	{
		DAZHairGroup[] array = hairGroups;
		foreach (DAZHairGroup dAZHairGroup in array)
		{
			if (dAZHairGroup.displayName == hairGroupName)
			{
				selectedHairGroup = dAZHairGroup;
			}
		}
	}

	public void CopyUI()
	{
		if (copyUIFrom != null)
		{
			color1DisplayNameText = copyUIFrom.color1DisplayNameText;
			color1Picker = copyUIFrom.color1Picker;
			color1Container = copyUIFrom.color1Container;
			color2DisplayNameText = copyUIFrom.color2DisplayNameText;
			color2Picker = copyUIFrom.color2Picker;
			color2Container = copyUIFrom.color2Container;
			color3DisplayNameText = copyUIFrom.color3DisplayNameText;
			color3Picker = copyUIFrom.color3Picker;
			color3Container = copyUIFrom.color3Container;
			param1DisplayNameText = copyUIFrom.param1DisplayNameText;
			param1Slider = copyUIFrom.param1Slider;
			param1DisplayNameTextAlt = copyUIFrom.param1DisplayNameTextAlt;
			param1SliderAlt = copyUIFrom.param1SliderAlt;
			param2DisplayNameText = copyUIFrom.param2DisplayNameText;
			param2Slider = copyUIFrom.param2Slider;
			param2DisplayNameTextAlt = copyUIFrom.param2DisplayNameTextAlt;
			param2SliderAlt = copyUIFrom.param2SliderAlt;
			param3DisplayNameText = copyUIFrom.param3DisplayNameText;
			param3Slider = copyUIFrom.param3Slider;
			param3DisplayNameTextAlt = copyUIFrom.param3DisplayNameTextAlt;
			param3SliderAlt = copyUIFrom.param3SliderAlt;
			param4DisplayNameText = copyUIFrom.param4DisplayNameText;
			param4Slider = copyUIFrom.param4Slider;
			param4DisplayNameTextAlt = copyUIFrom.param4DisplayNameTextAlt;
			param4SliderAlt = copyUIFrom.param4SliderAlt;
			param5DisplayNameText = copyUIFrom.param5DisplayNameText;
			param5Slider = copyUIFrom.param5Slider;
			param5DisplayNameTextAlt = copyUIFrom.param5DisplayNameTextAlt;
			param5SliderAlt = copyUIFrom.param5SliderAlt;
			param6DisplayNameText = copyUIFrom.param6DisplayNameText;
			param6Slider = copyUIFrom.param6Slider;
			param6DisplayNameTextAlt = copyUIFrom.param6DisplayNameTextAlt;
			param6SliderAlt = copyUIFrom.param6SliderAlt;
			param7DisplayNameText = copyUIFrom.param7DisplayNameText;
			param7Slider = copyUIFrom.param7Slider;
			param7DisplayNameTextAlt = copyUIFrom.param7DisplayNameTextAlt;
			param7SliderAlt = copyUIFrom.param7SliderAlt;
			param8DisplayNameText = copyUIFrom.param8DisplayNameText;
			param8Slider = copyUIFrom.param8Slider;
			param8DisplayNameTextAlt = copyUIFrom.param8DisplayNameTextAlt;
			param8SliderAlt = copyUIFrom.param8SliderAlt;
			textureGroup1Popup = copyUIFrom.textureGroup1Popup;
			textureGroup1PopupAlt = copyUIFrom.textureGroup1PopupAlt;
			textureGroup2Popup = copyUIFrom.textureGroup2Popup;
			textureGroup2PopupAlt = copyUIFrom.textureGroup2PopupAlt;
			textureGroup3Popup = copyUIFrom.textureGroup3Popup;
			textureGroup3PopupAlt = copyUIFrom.textureGroup3PopupAlt;
			textureGroup4Popup = copyUIFrom.textureGroup4Popup;
			textureGroup4PopupAlt = copyUIFrom.textureGroup4PopupAlt;
			textureGroup5Popup = copyUIFrom.textureGroup5Popup;
			textureGroup5PopupAlt = copyUIFrom.textureGroup5PopupAlt;
			textureGroup1Text = copyUIFrom.textureGroup1Text;
			textureGroup1TextAlt = copyUIFrom.textureGroup1TextAlt;
			textureGroup2Text = copyUIFrom.textureGroup2Text;
			textureGroup2TextAlt = copyUIFrom.textureGroup2TextAlt;
			textureGroup3Text = copyUIFrom.textureGroup3Text;
			textureGroup3TextAlt = copyUIFrom.textureGroup3TextAlt;
			textureGroup4Text = copyUIFrom.textureGroup4Text;
			textureGroup4TextAlt = copyUIFrom.textureGroup4TextAlt;
			textureGroup5Text = copyUIFrom.textureGroup5Text;
			textureGroup5TextAlt = copyUIFrom.textureGroup5TextAlt;
		}
	}

	private void ConnectCharacterMaterialOptionsUI()
	{
		if (_selectedCharacter != null)
		{
			DAZCharacterMaterialOptions componentInChildren = _selectedCharacter.GetComponentInChildren<DAZCharacterMaterialOptions>();
			if (componentInChildren != null)
			{
				componentInChildren.color1DisplayNameText = color1DisplayNameText;
				componentInChildren.color1Picker = color1Picker;
				componentInChildren.color1Container = color1Container;
				componentInChildren.color2DisplayNameText = color2DisplayNameText;
				componentInChildren.color2Picker = color2Picker;
				componentInChildren.color2Container = color2Container;
				componentInChildren.color3DisplayNameText = color3DisplayNameText;
				componentInChildren.color3Picker = color3Picker;
				componentInChildren.color3Container = color3Container;
				componentInChildren.param1DisplayNameText = param1DisplayNameText;
				componentInChildren.param1Slider = param1Slider;
				componentInChildren.param1DisplayNameTextAlt = param1DisplayNameTextAlt;
				componentInChildren.param1SliderAlt = param1SliderAlt;
				componentInChildren.param2DisplayNameText = param2DisplayNameText;
				componentInChildren.param2Slider = param2Slider;
				componentInChildren.param2DisplayNameTextAlt = param2DisplayNameTextAlt;
				componentInChildren.param2SliderAlt = param2SliderAlt;
				componentInChildren.param3DisplayNameText = param3DisplayNameText;
				componentInChildren.param3Slider = param3Slider;
				componentInChildren.param3DisplayNameTextAlt = param3DisplayNameTextAlt;
				componentInChildren.param3SliderAlt = param3SliderAlt;
				componentInChildren.param4DisplayNameText = param4DisplayNameText;
				componentInChildren.param4Slider = param4Slider;
				componentInChildren.param4DisplayNameTextAlt = param4DisplayNameTextAlt;
				componentInChildren.param4SliderAlt = param4SliderAlt;
				componentInChildren.param5DisplayNameText = param5DisplayNameText;
				componentInChildren.param5Slider = param5Slider;
				componentInChildren.param5DisplayNameTextAlt = param5DisplayNameTextAlt;
				componentInChildren.param5SliderAlt = param5SliderAlt;
				componentInChildren.param6DisplayNameText = param6DisplayNameText;
				componentInChildren.param6Slider = param6Slider;
				componentInChildren.param6DisplayNameTextAlt = param6DisplayNameTextAlt;
				componentInChildren.param6SliderAlt = param6SliderAlt;
				componentInChildren.param7DisplayNameText = param7DisplayNameText;
				componentInChildren.param7Slider = param7Slider;
				componentInChildren.param7DisplayNameTextAlt = param7DisplayNameTextAlt;
				componentInChildren.param7SliderAlt = param7SliderAlt;
				componentInChildren.param8DisplayNameText = param8DisplayNameText;
				componentInChildren.param8Slider = param8Slider;
				componentInChildren.param8DisplayNameTextAlt = param8DisplayNameTextAlt;
				componentInChildren.param8SliderAlt = param8SliderAlt;
				componentInChildren.textureGroup1Popup = textureGroup1Popup;
				componentInChildren.textureGroup1PopupAlt = textureGroup1PopupAlt;
				componentInChildren.textureGroup2Popup = textureGroup2Popup;
				componentInChildren.textureGroup2PopupAlt = textureGroup2PopupAlt;
				componentInChildren.textureGroup3Popup = textureGroup3Popup;
				componentInChildren.textureGroup3PopupAlt = textureGroup3PopupAlt;
				componentInChildren.textureGroup4Popup = textureGroup4Popup;
				componentInChildren.textureGroup4PopupAlt = textureGroup4PopupAlt;
				componentInChildren.textureGroup5Popup = textureGroup5Popup;
				componentInChildren.textureGroup5PopupAlt = textureGroup5PopupAlt;
				componentInChildren.textureGroup1Text = textureGroup1Text;
				componentInChildren.textureGroup1TextAlt = textureGroup1TextAlt;
				componentInChildren.textureGroup2Text = textureGroup2Text;
				componentInChildren.textureGroup2TextAlt = textureGroup2TextAlt;
				componentInChildren.textureGroup3Text = textureGroup3Text;
				componentInChildren.textureGroup3TextAlt = textureGroup3TextAlt;
				componentInChildren.textureGroup4Text = textureGroup4Text;
				componentInChildren.textureGroup4TextAlt = textureGroup4TextAlt;
				componentInChildren.textureGroup5Text = textureGroup5Text;
				componentInChildren.textureGroup5TextAlt = textureGroup5TextAlt;
				componentInChildren.InitUI();
			}
		}
	}

	public void InitComponents()
	{
		_physicsMeshes = GetComponentsInChildren<DAZPhysicsMesh>();
		_autoColliderBatchUpdaters = rootBones.GetComponentsInChildren<AutoColliderBatchUpdater>();
		_autoColliders = rootBones.GetComponentsInChildren<AutoCollider>();
		_setAnchorFromVertexComps = rootBones.GetComponentsInChildren<SetAnchorFromVertex>();
		_ignoreChildColliders = rootBones.GetComponentsInChildren<IgnoreChildColliders>();
		List<DAZCharacterMaterialOptions> list = new List<DAZCharacterMaterialOptions>();
		DAZCharacterMaterialOptions[] componentsInChildren = GetComponentsInChildren<DAZCharacterMaterialOptions>();
		DAZCharacterMaterialOptions[] array = componentsInChildren;
		foreach (DAZCharacterMaterialOptions dAZCharacterMaterialOptions in array)
		{
			DAZCharacter component = dAZCharacterMaterialOptions.GetComponent<DAZCharacter>();
			DAZSkinV2 component2 = dAZCharacterMaterialOptions.GetComponent<DAZSkinV2>();
			if (component == null && component2 == null)
			{
				list.Add(dAZCharacterMaterialOptions);
			}
		}
		_materialOptions = GetComponentsInChildren<DAZCharacterMaterialOptions>();
		_materialOptions = list.ToArray();
	}

	public void Init(bool force = false)
	{
		if (!wasInit || force)
		{
			wasInit = true;
			if (Application.isPlaying)
			{
				activeMorphSet = base.gameObject.AddComponent<DAZMorphSet>();
			}
			InitBones();
			InitComponents();
			InitClothingItems();
			InitHair();
			InitCharacters();
			SyncMaleAnatomy();
		}
	}

	protected IEnumerator PostEnable()
	{
		yield return null;
		ConnectSkin();
	}

	private void OnEnable()
	{
		Init();
		if (_selectedCharacter != null)
		{
			DAZCharacter dAZCharacter = _selectedCharacter;
			dAZCharacter.onLoadedHandlers = (DAZCharacter.OnLoaded)Delegate.Combine(dAZCharacter.onLoadedHandlers, new DAZCharacter.OnLoaded(OnCharacterLoaded));
		}
	}

	private void Start()
	{
		if (useAdvancedCollidersToggle != null)
		{
			useAdvancedCollidersToggle.isOn = _useAdvancedColliders;
			useAdvancedCollidersToggle.onValueChanged.AddListener(delegate
			{
				useAdvancedColliders = useAdvancedCollidersToggle.isOn;
			});
		}
		Init();
		if (Application.isPlaying)
		{
			selectedCharacter = startingCharacter;
			selectedHairGroup = startingHairGroup;
			ResetClothing();
		}
	}
}
