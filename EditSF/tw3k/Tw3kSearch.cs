using System;
using System.Collections.Generic;
using System.Diagnostics;
using EsfLibrary;

namespace EditSF.tw3k
{
    public class Tw3kSearch
    {
        private static readonly Dictionary<String, String> CeoMapNameKey = new Dictionary<string, string>();
        private static readonly Dictionary<String, String> CeoMapCodeKey = new Dictionary<String, String>();

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
                var template = persistentCharacter.AllNodes[10] as StringNode;

                var persistentCharacterFactionLink = persistentCharacter.Children[0];
                var characterArtSetInfo = persistentCharacter.Children[2];
                var persistentRetinue = persistentCharacter.Children[6];

                var faction = persistentCharacterFactionLink.AllNodes[0].ToString();
                var characterName = characterArtSetInfo.Values[0] as StringNode;
                //判断派系 1刘备
                if (!faction.Equals("2")) continue;
                //判断模板
                if (template == null) continue;
                String templateName = template.ToString();
//                if (templateName == "3k_main_template_historical_liu_bei_hero_earth")
                if (templateName.Contains("liu_bei"))
                {
                    Debug.WriteLine(i);
                    Debug.WriteLine(templateName);
                    ChangePersonality(esfFile, i, CeoCategory.personality, "", "", "",
                        "", "", "", "");
                }
            }
        }

        private static void InitCeoMap(EsfFile esfFile)
        {
            if (!(esfFile.RootNode is ParentNode campaignSaveGame)) return;
            CeoMapNameKey.Clear();
            CeoMapCodeKey.Clear();

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
//                String name = ownedCeo.AllNodes[0].ToString();
                String name2 = ceoDes.AllNodes[0].ToString();
                var code = ceo.AllNodes[5].ToString();

                if ("0" == code) continue;
                if ("17" == code)
                {
                    Debug.WriteLine("17号coe:" + name2);
                }

                if (name2.Contains("template"))
                {
                    Debug.WriteLine("模板[{0}", name2);
                }

                if (!CeoMapNameKey.ContainsKey(name2))
                {
                    CeoMapNameKey.Add(name2, code);
                    if (name2.StartsWith("3k_main_ceo_node_trait_personality") ||
                        name2.StartsWith("3k_ytr_ceo_node_trait_personality"))
                    {
                        Debug.WriteLine(name2);
                    }
                }

//                else
//                {
//                    Debug.WriteLine("已存在[{0}]/[{1}]，未添加[{2}]/[{3}]", name2, CeoMapNameKey[name2], name2, code);
//                }

                if (!CeoMapCodeKey.ContainsKey(code))
                {
                    CeoMapCodeKey.Add(code, name2);
                }

//                else
//                {
//                    Debug.WriteLine("已存在[{0}]/[{1}]，未添加[{2}]/[{3}]", code, CeoMapCodeKey[code], code, name2);
//                }
            }

            Debug.WriteLine("加载name数量：{0}", CeoMapNameKey.Count);
            Debug.WriteLine("加载code数量：{0}", CeoMapCodeKey.Count);
        }

        private static void ChangePersonality(EsfFile esfFile, int characterIndex, String ceoCategory,
            String personality1, String personality2, String personality3, String personality4, String personality5,
            String personality6, String personality7)
        {
            if (!(esfFile.RootNode is ParentNode campaignSaveGame)) return;
            var compressedData = campaignSaveGame.Children[3];
            var campaignEnv = compressedData.Children[3];
            var campaignModel = campaignEnv.Children[6];

            var ceoSystemManagement = campaignModel.Children[7];
            var ceoSystemModel = ceoSystemManagement.Children[0];
            var ceoSystemCeos = ceoSystemManagement.Children[1];
            var charactersConnectedCeoManagement = ceoSystemModel.Children[2];
            var character = charactersConnectedCeoManagement.Children[characterIndex];
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
                    equipmentSlotsBlock = block;
                }
            }

            ChangePersonality(equipmentSlotsBlock, ceoPoolBlock, personality1, personality2, personality3,
                personality4, personality5, personality6, personality7);
        }

        private static void ChangePersonality(ParentNode equipmentSlotsBlock, ParentNode ceoPoolBlock, String code1,
            String code2, String code3, String code4, String code5, String code6, String code7)
        {
            var blockName = equipmentSlotsBlock.AllNodes[0].ToString();
            if (blockName != CeoCategory.personality) return;
            var equipmentCategoryManager = equipmentSlotsBlock.Children[0];
            var ceoBlock = equipmentCategoryManager.Children[0];
            // 特性1
            var a = ceoBlock.Children[0].Children[0].AllNodes[0].ToString();
            // 特性2
            var b = ceoBlock.Children[1].Children[0].AllNodes[0].ToString();
            // 特性3
            var c = ceoBlock.Children[2].Children[0].AllNodes[0].ToString();
            // 特性4
            var d = ceoBlock.Children[3].Children[0].AllNodes[0].ToString();
            // 特性5
            var e = ceoBlock.Children[4].Children[0].AllNodes[0].ToString();
            // 特性6
            var f = ceoBlock.Children[5].Children[0].AllNodes[0].ToString();
            // 特性7
            var g = ceoBlock.Children[6].Children[0].AllNodes[0].ToString();
            if (CeoMapCodeKey.ContainsKey(a)) Debug.WriteLine("{0}  {1}", a, CeoMapCodeKey[a]);
            if (CeoMapCodeKey.ContainsKey(b)) Debug.WriteLine("{0}  {1}", b, CeoMapCodeKey[b]);
            if (CeoMapCodeKey.ContainsKey(c)) Debug.WriteLine("{0}  {1}", c, CeoMapCodeKey[c]);
            if (CeoMapCodeKey.ContainsKey(d)) Debug.WriteLine("{0}  {1}", d, CeoMapCodeKey[d]);
            if (CeoMapCodeKey.ContainsKey(e)) Debug.WriteLine("{0}  {1}", e, CeoMapCodeKey[e]);
            if (CeoMapCodeKey.ContainsKey(f)) Debug.WriteLine("{0}  {1}", f, CeoMapCodeKey[f]);
            if (CeoMapCodeKey.ContainsKey(g)) Debug.WriteLine("{0}  {1}", g, CeoMapCodeKey[g]);
            Debug.WriteLine("-----------------------------");
            var map = CeoMapNameKey;
            String codeHeavenHonest = map["3k_ytr_ceo_node_trait_personality_heaven_honest"];
            Debug.WriteLine("{0}  {1}", codeHeavenHonest, CeoMapCodeKey[codeHeavenHonest]);
        }
    }
}