using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    static List<string> names;

    public static void Initialize() {
        names = new List<string>();
    
        names.Add("Andoe");	names.Add("Niry");	names.Add("Gethi");	names.Add("Ene");	names.Add("Assol");	names.Add("Onun");	names.Add("Gamec");	names.Add("Nossu");
        names.Add("Volni");	names.Add("Esu");	names.Add("Uliok");	names.Add("Rhilnush");	names.Add("Kiozon");	names.Add("Linnas");	names.Add("Uthmid");	names.Add("Weldy");
        names.Add("Zezob");	names.Add("Ilgral");	names.Add("Phiathon");	names.Add("Phimnar");	names.Add("Muroes");	names.Add("Selar");	names.Add("Tumu");	names.Add("Walel");
        names.Add("Gelnis");	names.Add("Shidru");	names.Add("Diassi");	names.Add("Murshal");	names.Add("Kulnun");	names.Add("Lasal");	names.Add("Nulnu");	names.Add("Phamru");
        names.Add("Daldu");	names.Add("Renmu");	names.Add("Phiagrolm");	names.Add("Jella");	names.Add("Zulab");	names.Add("Lullosh");	names.Add("Wardi");	names.Add("Shynmo");
        names.Add("Vordoecy");	names.Add("Shynronna");	names.Add("Nigresseb");	names.Add("Omulgra");	names.Add("Vososseln");	names.Add("Shernylni");	names.Add("Wiolmoesa");	names.Add("Phymrilma");
        names.Add("Phathmendolm");	names.Add("Renamron");	names.Add("Muthmime");	names.Add("Hanunras");	names.Add("Larialnu");	names.Add("Rhorlano");	names.Add("Vamodra");	names.Add("Resissys");
        names.Add("Zosilneb");	names.Add("Rillosse");	names.Add("Arana");	names.Add("Onnutha");	names.Add("Memnerdulm");	names.Add("Orune");	names.Add("Kondessub");	names.Add("Oshasa");
        names.Add("Ocilmir");	names.Add("Zidrurshe");	names.Add("Werdossik");	names.Add("Shanralla");	names.Add("Phothimdaln");	names.Add("Unarnel");	names.Add("Worusos"); names.Add("Sonmarnu");
        names.Add("Gylese"); names.Add("Zyrliro"); names.Add("Kedriathmac"); names.Add("Chalnony"); names.Add("Jirlutholm"); names.Add("Phamnullo"); names.Add("Tardino"); names.Add("Rimenil");
    }

    public static string GetRandomCharacterName() {
        int index = Random.Range(0, names.Count - 1);
        return names[index];
    }

}
