using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EsfLibrary;

namespace EditSF.tw3k
{
    public class Tw3kSearch
    {
        private static readonly Dictionary<String, ParentNode> CeoMapNameKey = new Dictionary<string, ParentNode>();
        private static readonly Dictionary<String, String> CeoMapEquipmentCodeKey = new Dictionary<String, String>();

        public static void SearchFaction(EsfFile esfFile)
        {
            if (!(esfFile.RootNode is ParentNode campaignSaveGame)) return;

            InitCeoMap(esfFile);

            var compressedData = campaignSaveGame.Children[3];
            var campaignEnv = compressedData.Children[3];
            var campaignModel = campaignEnv.Children[6];

            var world = campaignModel.Children[5];
            var characterGenerator = world.Children[12];
            var persistentCharacterStorage = characterGenerator.Children[0];
            var characters = persistentCharacterStorage.Children[0];

            for (var i = 0; i < characters.Children.Count - 1; i++)
            {
                var character = characters.Children[i];

                var persistentCharacter = character.Children[0];
                var characterIndex = persistentCharacter.AllNodes[0].ToString();
                var templateName = persistentCharacter.AllNodes[10].ToString();

                var persistentCharacterFactionLink = persistentCharacter.Children[0];
                var characterArtSetInfo = persistentCharacter.Children[2];
                var persistentRetinue = persistentCharacter.Children[6];

                var faction = persistentCharacterFactionLink.AllNodes[0].ToString();
                var characterName = characterArtSetInfo.Values[0] as StringNode;
                //判断派系 1刘备 4袁绍
                if (!faction.Equals("2")) continue;
                //判断模板
//                String templateName = template.ToString();
//                if (templateName.Contains("xu_you"))
//                {
                Debug.WriteLine(characterIndex);
                Debug.WriteLine(templateName);
                ChangePersonality(esfFile, characterIndex, CeoCategory.personality, "", "", "",
                    "3k_ytr_ceo_trait_personality_heaven_honest", "", "", "");
//                }
            }
        }

        private static void InitCeoMap(EsfFile esfFile)
        {
            if (!(esfFile.RootNode is ParentNode campaignSaveGame)) return;
            CeoMapNameKey.Clear();
            CeoMapEquipmentCodeKey.Clear();

            var compressedData = campaignSaveGame.Children[3];
            var campaignEnv = compressedData.Children[3];
            var campaignModel = campaignEnv.Children[6];
            var ceoSystemManagement = campaignModel.Children[7];
            var ceoSystemCeos = ceoSystemManagement.Children[1];
            var allOwnedCeos = ceoSystemCeos.Children[0];
            Debug.WriteLine("总CEO：{0}", allOwnedCeos.Children.Count);
            for (var i = 0; i < allOwnedCeos.Children.Count; i++)
            {
                var ownedCeo = allOwnedCeos.Children[i];
                var ceo = ownedCeo.Children[0];
                var ceoDes = ownedCeo.Children[1];
                String name = ownedCeo.AllNodes[0].ToString();
                String name2 = ceoDes.AllNodes[0].ToString();
                var ceoCode = ceo.AllNodes[0].ToString();
                var equipmentCode = ceo.AllNodes[5].ToString();

                if ("0" == equipmentCode) continue;
                if (!CeoMapNameKey.ContainsKey(name))
                {
                    CeoMapNameKey.Add(name, ceo);
                    if (name.StartsWith("3k_main_ceo_trait_personality") ||
                        name.StartsWith("3k_ytr_ceo_trait_personality"))
                    {
                        //Debug.WriteLine(name);
                    }
                }

//                else
//                {
//                    Debug.WriteLine("已存在[{0}]/[{1}]，未添加[{2}]/[{3}]", name2, CeoMapNameKey[name2], name2, code);
//                }

                if (!CeoMapEquipmentCodeKey.ContainsKey(equipmentCode))
                {
                    CeoMapEquipmentCodeKey.Add(equipmentCode, name);
                }

//                else
//                {
//                    Debug.WriteLine("已存在[{0}]/[{1}]，未添加[{2}]/[{3}]", code, CeoMapCodeKey[code], code, name2);
//                }
            }

            Debug.WriteLine("加载name数量：{0}", CeoMapNameKey.Count);
            Debug.WriteLine("加载code数量：{0}", CeoMapEquipmentCodeKey.Count);
        }

        private static bool ChangePersonality(EsfFile esfFile, String characterIndex, String ceoCategory,
            String personality1, String personality2, String personality3, String personality4, String personality5,
            String personality6, String personality7)
        {
            if (!(esfFile.RootNode is ParentNode campaignSaveGame)) return false;
            var compressedData = campaignSaveGame.Children[3];
            var campaignEnv = compressedData.Children[3];
            var campaignModel = campaignEnv.Children[6];

            var ceoSystemManagement = campaignModel.Children[7];
            var ceoSystemModel = ceoSystemManagement.Children[0];
            var ceoSystemCeos = ceoSystemManagement.Children[1];
            var charactersConnectedCeoManagement = ceoSystemModel.Children[2];
            var charactersConnectedCeoManagementChildren = charactersConnectedCeoManagement.Children;
            ParentNode character = null;
            foreach (var c in charactersConnectedCeoManagementChildren)
            {
                if (c.AllNodes[0].ToString().Equals(characterIndex))
                {
                    character = c;
                }
            }

            if (character == null) return false;
            var management = character.Children[0];
            //装备管理
            var equipmentManager = management.Children[1];
            var equipmentSlotsBlocks = equipmentManager.Children[0];
            //ceo关系管理
            var ceoOwnerShipManager = management.Children[3];
            var ceoPoolBlocks = ceoOwnerShipManager.Children[0];
            //遍历获取装备槽和ceo池
            ParentNode equipmentSlotsBlock = null;
            ParentNode ceoPoolBlock = null;
            foreach (var block in equipmentSlotsBlocks.Children)
            {
                String blockCategory = block.AllNodes[0].ToString();
                if (blockCategory == ceoCategory)
                {
                    equipmentSlotsBlock = block;
                }
            }

            foreach (var block in ceoPoolBlocks.Children)
            {
                String blockCategory = block.AllNodes[0].ToString();
                if (blockCategory == ceoCategory)
                {
                    ceoPoolBlock = block;
                }
            }

            return ChangePersonality(equipmentSlotsBlock, ceoPoolBlock, personality1, personality2, personality3,
                personality4, personality5, personality6, personality7);
        }

        private static bool ChangePersonality(ParentNode equipmentSlotsBlock, ParentNode ceoPoolBlock,
            String personality1, String personality2, String personality3, String personality4, String personality5,
            String personality6, String personality7)
        {
            var equipmentSlotsBlockName = equipmentSlotsBlock.AllNodes[0].ToString();
            if (equipmentSlotsBlockName != CeoCategory.personality) return false;
            var equipmentCategoryManager = equipmentSlotsBlock.Children[0];
            var equipmentSlotsBlockSlots = equipmentCategoryManager.Children[0];

            var ceoPoolBlockName = ceoPoolBlock.AllNodes[0].ToString();
            if (ceoPoolBlockName != CeoCategory.personality) return false;
            var ceoPool = ceoPoolBlock.Children[0];
            //RecordArrayNode
            var ceoMapBlocks = ceoPool.Children[0];
            //RecordEntryNode
            RecordEntryNode c0 = ceoMapBlocks.Children[0] as RecordEntryNode;
            if (c0 == null) return false;
            if (personality4 != null)
            {
                var c3 = c0.CreateCopy() as RecordEntryNode;
                if (c3?.AllNodes[0] == null) return false;
                var c3Name = c3.AllNodes[0] as StringNode;
                c3Name.Value = personality4;
                var c3CeoBlocks = c3.AllNodes[1] as RecordArrayNode;
                var c3Code = c3CeoBlocks.Children[0].AllNodes[0] as OptimizedUIntNode; //12 15 14
                c3Code.Value = (CeoMapNameKey[personality4].AllNodes[0] as OptimizedUIntNode).Value;
                ceoMapBlocks.Value.Insert(3, c3);
                var e3Code =
                    equipmentSlotsBlockSlots.Children[3].Children[0].AllNodes[0] as OptimizedUIntNode; //14 15 16
                e3Code.Value = (CeoMapNameKey[personality4].AllNodes[5] as OptimizedUIntNode).Value;
            }


            // 特性1
            var a = equipmentSlotsBlockSlots.Children[0].Children[0].AllNodes[0].ToString();
            // 特性2
            var b = equipmentSlotsBlockSlots.Children[1].Children[0].AllNodes[0].ToString();
            // 特性3
            var c = equipmentSlotsBlockSlots.Children[2].Children[0].AllNodes[0].ToString();
//            // 特性4
//            var d = equipmentSlotsBlockSlots.Children[3].Children[0].AllNodes[0].ToString();
//            // 特性5
//            var e = equipmentSlotsBlockSlots.Children[4].Children[0].AllNodes[0].ToString();
//            // 特性6
//            var f = equipmentSlotsBlockSlots.Children[5].Children[0].AllNodes[0].ToString();
//            // 特性7
//            var g = equipmentSlotsBlockSlots.Children[6].Children[0].AllNodes[0].ToString();
            if (CeoMapEquipmentCodeKey.ContainsKey(a)) Debug.WriteLine("{0}  {1}", a, CeoMapEquipmentCodeKey[a]);
            if (CeoMapEquipmentCodeKey.ContainsKey(b)) Debug.WriteLine("{0}  {1}", b, CeoMapEquipmentCodeKey[b]);
            if (CeoMapEquipmentCodeKey.ContainsKey(c)) Debug.WriteLine("{0}  {1}", c, CeoMapEquipmentCodeKey[c]);
//            if (CeoMapEquipmentCodeKey.ContainsKey(d)) Debug.WriteLine("{0}  {1}", d, CeoMapEquipmentCodeKey[d]);
//            if (CeoMapEquipmentCodeKey.ContainsKey(e)) Debug.WriteLine("{0}  {1}", e, CeoMapEquipmentCodeKey[e]);
//            if (CeoMapEquipmentCodeKey.ContainsKey(f)) Debug.WriteLine("{0}  {1}", f, CeoMapEquipmentCodeKey[f]);
//            if (CeoMapEquipmentCodeKey.ContainsKey(g)) Debug.WriteLine("{0}  {1}", g, CeoMapEquipmentCodeKey[g]);
            Debug.WriteLine("-----------------------------");
//            var map = CeoMapNameKey;
//            String codeHeavenHonest = map["3k_ytr_ceo_trait_personality_heaven_honest"];
//            Debug.WriteLine("{0}  {1}", codeHeavenHonest, CeoMapCodeKey[codeHeavenHonest]);

            return true;
        }

        public static T DeepCopyByReflection<T>(T obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return (T) retval;
        }
    }
}