using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mg.pummelz
{



    [System.Serializable]
    public class MGPumCampaign : IEnumerable<MGPumEncounter>
    {
        private static MGPumCampaign instance;

        public List<MGPumEncounter> encounters;

        public Dictionary<string, MGPumEncounter> extraEncounters;

        protected string campaignID;

        private Dictionary<char, string> unitDictionary;
        private Dictionary<char, MGPumField.Terrain> terrainDictionary;

        private void initializeDictionaries()
        {
            //
            // Units
            //
            unitDictionary = new Dictionary<char, string>();
            unitDictionary.Add('p', MGPumBaseSet.BELLIE);
            unitDictionary.Add('h', MGPumBaseSet.HOPPEL);
            unitDictionary.Add('s', MGPumBaseSet.SNEIP);
            unitDictionary.Add('w', MGPumBaseSet.WOLLI);
            unitDictionary.Add('c', MGPumBaseSet.CHILLY);
            unitDictionary.Add('k', MGPumBaseSet.KILLY);
            unitDictionary.Add('b', MGPumBaseSet.BUMMZ);
            unitDictionary.Add('z', MGPumBaseSet.CZAREMIR);
            unitDictionary.Add('u', MGPumBaseSet.LINK);
            unitDictionary.Add('m', MGPumBaseSet.MAMPFRED);
            unitDictionary.Add('a', MGPumBaseSet.SPOT);
            unitDictionary.Add('f', MGPumBaseSet.FLUFFY);
            unitDictionary.Add('y', MGPumBaseSet.BUFFY);
            unitDictionary.Add('l', MGPumBaseSet.HALEY);

            //
            // Terrain
            //
            terrainDictionary = new Dictionary<char, MGPumField.Terrain>();
            terrainDictionary.Add('E', MGPumField.Terrain.Earth);
            terrainDictionary.Add('G', MGPumField.Terrain.Grass);
            terrainDictionary.Add('S', MGPumField.Terrain.Sand);
            terrainDictionary.Add('W', MGPumField.Terrain.Water);
            terrainDictionary.Add('M', MGPumField.Terrain.Mountain);
            terrainDictionary.Add('L', MGPumField.Terrain.Lava);
            terrainDictionary.Add('I', MGPumField.Terrain.Ice);
        }
        
        public static MGPumCampaign getInstance()
        {
            if (instance == null)
            {
                instance = new MGPumCampaign("main");
            }
            return instance;
        }

        protected void addEncounter(MGPumEncounter encounter)
        {
            addEncounter(encounter, true);
        }

        protected void addExtraEncounter(MGPumEncounter encounter)
        {
            addEncounter(encounter, false);
        }

        public MGPumEncounter getEncounter(string key)
        {
            if (!extraEncounters.ContainsKey(key))
            {
                Debug.LogError("Unknown encounter " + key);
            }
            return extraEncounters[key];
        }

        protected void addEncounter(MGPumEncounter encounter, bool regularEncounter)
        {
            encounter.unitDictionary = this.unitDictionary;
            encounter.terrainDictionary = this.terrainDictionary;

            if (regularEncounter)
            {
                this.encounters.Add(encounter);
            }

            this.extraEncounters.Add(encounter.id, encounter);
        }

        public IEnumerator<MGPumEncounter> GetEnumerator()
        {
            return ((IEnumerable<MGPumEncounter>)encounters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<MGPumEncounter>)encounters).GetEnumerator();
        }

        public const string TEST = "TEST";

        //three without special abilities
        public const string FIRST = "FIRST";
        public const string FOUR_CORNERS = "FOUR_CORNERS";
        public const string BIG_BRAWL = "BIG_BRAWL";

        //basic special abilities
        public const string CHILLORDER = "CHILLORDER";
        public const string HERDING = "HERDING";
        public const string KINGSLAYER = "KINGSLAYER";

        //advanced/extreme encounters
        public const string BURNING_FUSE = "BURNING_FUSE";
        //public const string BUMMPITZ = "PUM_BUMMPITZ";
        public const string A_LINK_TO_THE_BLAST = "A_LINK_TO_THE_BLAST";
        public const string GREAT_LAKE = "GREAT_LAKE";
        
        //everything at once 
        public const string GRAND_CANYON = "GRAND_CANYON";
        
        //old levels
        //public const string CHESS = "CHESS";



        protected MGPumCampaign()
        {

        }


        private MGPumCampaign(string campaignID)
        {
            this.campaignID = campaignID;

            encounters = new List<MGPumEncounter>();
            extraEncounters = new Dictionary<string, MGPumEncounter>();

            initializeDictionaries();

            //Student Encounters
            {


                {
                    MGPumEncounter encounter = new MGPumEncounter(FIRST, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "..WHS...",//0
                        "..PPP...",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        "...ppp..",//6
                        "...shw.." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "GGGGGGGG",//0
                        "GGGGGGGG",//1
                        "GGGGGGGG",//2
                        "GGGGGGGG",//3
                        "GGGGGGGG",//4
                        "GGGGGGGG",//5
                        "GGGGGGGG",//6
                        "GGGGGGGG" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }


                {
                    MGPumEncounter encounter = new MGPumEncounter(FOUR_CORNERS, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "swp..PHS",//0
                        "hp....PW",//1
                        "p......P",//2
                        "........",//3
                        "........",//4
                        "P......p",//5
                        "WP....ph",//6
                        "SHP..pws" //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SSSWWSSS",//0
                        "SSSWWSSS",//1
                        "GGSSSSGG",//2
                        "WWGSSGWW",//3
                        "WWGSSGWW",//4
                        "GGSGGSGG",//5
                        "SSGWWGSS",//6
                        "SSGWWGSS" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                {
                    MGPumEncounter encounter = new MGPumEncounter(BIG_BRAWL, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "hshshshs",//0
                        "wpwpwpwp",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        "PWPWPWPW",//6
                        "SHSHSHSH" //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "EEEEEEEE",//0
                        "EEEEEEEE",//1
                        "LEEEEEEL",//2
                        "LLEEEELL",//3
                        "LLEEEELL",//4
                        "LEEEEEEL",//5
                        "EEEEEEEE",//6
                        "EEEEEEEE" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }


                {
                    MGPumEncounter encounter = new MGPumEncounter(HERDING, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "PLPYPLPY",//0
                        "PPPPPPPP",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        "pppppppp",//6
                        "yplpyplp" //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "GGGGGGGG",//0
                        "GGGGGEGG",//1
                        "GGGEEGGG",//2
                        "GGEEEEEG",//3
                        "GGGGEEEG",//4
                        "GGEEEEGG",//5
                        "GGGEEGGG",//6
                        "GGGGGGGG" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                {
                    MGPumEncounter encounter = new MGPumEncounter(CHILLORDER, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        ".SMAAMS.",//0
                        ".CPHHPC.",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        ".cphhpc.",//6
                        ".smaams." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "GGSSSSGG",//0
                        "GGSSSSGG",//1
                        "SSSLLSSS",//2
                        "SSLLLLSS",//3
                        "SSLLLLSS",//4
                        "SSSLLSSS",//5
                        "GGSSSSGG",//6
                        "GGSSSSGG" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                

                {
                    MGPumEncounter encounter = new MGPumEncounter(KINGSLAYER, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "..PLYLP.",//0
                        "..PWZHP.",//1
                        "...PPP..",//2
                        "........",//3
                        "........",//4
                        "..ppp...",//5
                        ".pwzhp..",//6
                        ".plylp.." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SESESESE",//0
                        "ESESESES",//1
                        "SESESESE",//2
                        "ESESESES",//3
                        "SESESESE",//4
                        "ESESESES",//5
                        "SESESESE",//6
                        "ESESESES" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }


                {
                    MGPumEncounter encounter = new MGPumEncounter(BURNING_FUSE, 1);

                    encounter.battlegrounds = new string[] {
                        //01234567
                        ".PBLLBP.",//0
                        ".BCCCCB.",//1
                        "..BPPB..",//2
                        "........",//3
                        "........",//4
                        "..bppb..",//5
                        ".bccccb.",//6
                        ".pbllbp." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SSSSEEEE",//0
                        "SEEEEESS",//1
                        "SSSSEEES",//2
                        "SSEEESSS",//3
                        "SEEESSES",//4
                        "SSSEEEES",//5
                        "SESSEESS",//6
                        "SEEEEESS" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }


              

                {
                    MGPumEncounter encounter = new MGPumEncounter(A_LINK_TO_THE_BLAST, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "...UUUBZ",//0
                        "......BB",//1
                        ".......U",//2
                        "u......U",//3
                        "u......U",//4
                        "u.......",//5
                        "bb......",//6
                        "zbuuu..." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "WWGGGGGG",//0
                        "WWWGGGGG",//1
                        "GWLGGGGG",//2
                        "GGGLGGGG",//3
                        "GGGGLGGG",//4
                        "GGGGGLWG",//5
                        "GGGGGWWW",//6
                        "GGGGGGWW" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                {
                    MGPumEncounter encounter = new MGPumEncounter(GREAT_LAKE, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "....YMSH",//0
                        ".....FAS",//1
                        "......FM",//2
                        ".......L",//3
                        "l.......",//4
                        "mf......",//5
                        "saf.....",//6
                        "hsmy...." //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SSGGGGSS",//0
                        "SGWWWWGS",//1
                        "GWWWWLWG",//2
                        "GWWGGWWG",//3
                        "GWWGGWWG",//4
                        "GWLWWWWG",//5
                        "SGWWWWGS",//6
                        "SSGGGGSS" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                {
                    MGPumEncounter encounter = new MGPumEncounter(GRAND_CANYON, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "hcyfzach",//0
                        "mwsbbswm",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        "MWSBBSWM",//6
                        "HCAZFYCH",//7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "GGEEEEGG",//0
                        "GESSESEG",//1
                        "ESSESSSE",//2
                        "WWWWWWWW",//3
                        "WWWWWWWW",//4
                        "ESSESSSE",//5
                        "GESSESEG",//6
                        "GGEEEEGG" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }

                //test encounters
                //{
                //    MGPumEncounter encounter = new MGPumEncounter("uiuiuiui", 1);

                //    encounter.battlegrounds = new string[] {
                //       //01234567
                //        "...TT...",//0
                //        "t..CC..t",//1
                //        "h..cc..H",//2
                //        "m......M",//3
                //        "p......P",//4
                //        "........",//5
                //        "........",//6
                //        "........" //7
                //    };

                //    {//always the same
                //        addEncounter(encounter);
                //    }
                //}
                
                {
                    MGPumEncounter encounter = new MGPumEncounter("template", 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "........",//0
                        "........",//1
                        "........",//2
                        "........",//3
                        "........",//4
                        "........",//5
                        "........",//6
                        "........" //7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SSSSSSSS",//0
                        "SSSSSSSS",//1
                        "SSSSSSSS",//2
                        "SSSSSSSS",//3
                        "SSSSSSSS",//4
                        "SSSSSSSS",//5
                        "SSSSSSSS",//6
                        "SSSSSSSS" //7
                    };

                    {//always the same
                        //addEncounter(encounter);
                    }
                }

                {
                    MGPumEncounter encounter = new MGPumEncounter(TEST, 1);

                    encounter.battlegrounds = new string[] {
                       //01234567
                        "aa.ss.ff",//0
                        "aa....ff",//1
                        "...SS...",//2
                        "...mm.z.",//3
                        ".Z.MM...",//4
                        "...ss...",//5
                        "FF....AA",//6
                        "FF.SS.AA",//7
                    };

                    encounter.terrain = new string[] {
                       //01234567
                        "SSWMMWSS",//0
                        "SSWMMWSS",//1
                        "SSSGGSSS",//2
                        "LSGGGGSL",//3
                        "LSGGGGSL",//4
                        "SSSGGSSS",//5
                        "SSWMMWSS",//6
                        "SSWMMWSS" //7
                    };

                    {//always the same
                        addEncounter(encounter);
                    }
                }
            }
        }


    }


}




