﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MeterControllerScript : MonoBehaviour {

    public GameObject BarPrefab;
    
    List<Bar> m_bars;
    List<Entry> m_entries;

    public class Entry {

        public Entry(string n, int i)
        {
            m_name = n;
            m_index = i;
        }

        string m_name;
        int m_index;
        public int Amount;

        public string Name { get { return m_name; } }
        public int Index { get { return m_index; } }
    }

    public class Bar
    {
        public Bar(MeterBarScript sl, int i)
        {
            m_barScript = sl;
            m_index = i;
        }

        MeterBarScript m_barScript;
        int m_index;
        public Entry CurrentEntry;

        public MeterBarScript BarScript { get { return m_barScript; } }
        public int Index { get { return m_index; } }
    }
    
    public void Initialize(int xPos, int yPos, int height, int width, int numDisplayedBars) {
        m_bars = new List<Bar>();
        for (int i = 0; i < numDisplayedBars; i++)
        {
            GameObject temp = GameObject.Instantiate(BarPrefab);
            temp.transform.SetParent(this.transform);
            temp.transform.SetPositionAndRotation(new Vector3(xPos, yPos - (height*i)/2, 0), Quaternion.identity);
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            m_bars.Add(new Bar(temp.GetComponent <MeterBarScript>(), i));
            m_bars[i].BarScript.UpdateEntry(new Entry("None", 0));
            m_bars[i].BarScript.NameText.resizeTextForBestFit = true;
            m_bars[i].BarScript.AmountText.resizeTextForBestFit = true;
        }

        BarPrefab.SetActive(false);
    }

    public void CreateEntriesFromRaid(List<Raider> raid)
    {
        m_entries = new List<Entry>();
        for (int i = 0; i < raid.Count; i++)
        {
            m_entries.Add(new Entry(raid[i].GetName(), i));
        }

        if (raid.Count < m_bars.Count)
            m_bars.RemoveRange(raid.Count - 1, m_bars.Count - raid.Count);

        for (int i = 0; i < m_bars.Count; i++)
        {
            m_bars[i].CurrentEntry = m_entries[i];
        }
    }

    public void AddAmountToEntry(string name, int index, int amount) {
        Entry entry = m_entries.Find(x => x.Name == name && x.Index == index);
        if (entry != null) {
            entry.Amount += Mathf.Abs(amount);
            CheckForBarUpdate(entry);
        }

        return;
        Debug.Log("1: " + m_entries[0].Name + ": " + m_entries[0].Amount + "\n" +
            "2: " + m_entries[1].Name + ": " + m_entries[1].Amount + "\n" +
            "3: " + m_entries[2].Name + ": " + m_entries[2].Amount + "\n" +
            "4: " + m_entries[3].Name + ": " + m_entries[3].Amount + "\n" +
            "5: " + m_entries[4].Name + ": " + m_entries[4].Amount + "\n" +
            "6: " + m_entries[5].Name + ": " + m_entries[5].Amount + "\n" +
            "7: " + m_entries[6].Name + ": " + m_entries[6].Amount + "\n" +
            "8: " + m_entries[7].Name + ": " + m_entries[7].Amount + "\n" +
            "9: " + m_entries[8].Name + ": " + m_entries[8].Amount + "\n" +
            "10: " + m_entries[9].Name + ": " + m_entries[9].Amount + "\n" +
            "11: " + m_entries[10].Name + ": " + m_entries[10].Amount + "\n" +
            "12: " + m_entries[11].Name + ": " + m_entries[11].Amount + "\n");
    }
    

    void CheckForBarUpdate(Entry updatedEntry) {


        //The bruteforce version

        int max = 0;
        for (int i = 0; i < m_entries.Count; i++)
        {
            if (m_entries[i].Amount > max)
                max = m_entries[i].Amount;
        }

        UpdateNewMax(max);

        m_entries.Sort(delegate (Entry x, Entry y)
        {
            if (x.Amount > y.Amount)
                return -1;
            else return 1;
        });

        for (int i = 0; i < m_bars.Count; i++)
        {
            m_bars[i].CurrentEntry = m_entries[i];
            m_bars[i].BarScript.UpdateEntry(m_entries[i]);
        }


        #region the smart version that we eventually want

        // Early out if we're not even making the list
        /*if (m_bars[m_bars.Count - 1].CurrentEntry.Amount > updatedEntry.Amount)
            return;
        */
        //Find the new spot on the list for us.
        /* int newSpot;
         for (newSpot = m_bars.Count - 1; newSpot > 0; newSpot--)
         {
             //Debug.Log("m_bars[" + newSpot + "].CurrentEntry.Amount: " + m_bars[newSpot].CurrentEntry.Amount);
             //Debug.Log("updatedEntry.Amount: " + updatedEntry.Amount);
             if (m_bars[newSpot].CurrentEntry.Amount >= updatedEntry.Amount)
                 break;
         }*/

        //If our new spot in number 1, update the maxvalue for all bars
        /*
        if (newSpot == 0)
            UpdateNewMax(updatedEntry.Amount);
            */

        // If we didnt move a spot, just update the value and return
        /*if (m_bars[newSpot].CurrentEntry.Name == updatedEntry.Name && m_bars[newSpot].CurrentEntry.Index == updatedEntry.Index) {
            m_bars[newSpot].CurrentEntry.Amount = updatedEntry.Amount;
            m_bars[newSpot].BarScript.UpdateEntry(updatedEntry);
            return;
        }*/

        //Otherwise, move other people down
        /* for (int i = m_bars.Count-1; i > newSpot; i--)
         {
             if (i != m_bars.Count - 1)
             {
                 m_bars[i+1].CurrentEntry = m_bars[i].CurrentEntry;
                 m_bars[i + 1].BarScript.UpdateEntry(m_bars[i + 1].CurrentEntry);
             }
         }*/

        //m_bars[newSpot].CurrentEntry = updatedEntry;
        //m_bars[newSpot].BarScript.UpdateEntry(updatedEntry);
        #endregion
    }

    void UpdateNewMax(int newMax)
    {
        for (int i = 0; i < m_bars.Count; i++)
        {
            m_bars[i].BarScript.UpdateMax(newMax);
        }
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
