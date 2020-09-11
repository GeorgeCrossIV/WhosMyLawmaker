using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lawmakers.Models
{

    public class USGovernment
    {
        public Lawmaker[] Lawmakers { get; set; }
    }

    public class LawmakerDocument
    {
        public string documentId { get; set; }
        public Lawmaker data { get; set; }
    }

    public class Lawmaker
    {
        public Id id { get; set; }
        public Name name { get; set; }
        public Bio bio { get; set; }
        public Term[] terms { get; set; }
        public Leadership_Roles[] leadership_roles { get; set; }
        public Other_Names[] other_names { get; set; }
        public Family[] family { get; set; }
    }

    public class Id
    {
        public string bioguide { get; set; }
        public string thomas { get; set; }
        public string lis { get; set; }
        public int govtrack { get; set; }
        public string opensecrets { get; set; }
        public int votesmart { get; set; }
        public string[] fec { get; set; }
        public int cspan { get; set; }
        public string wikipedia { get; set; }
        public long house_history { get; set; }
        public string ballotpedia { get; set; }
        public int maplight { get; set; }
        public int icpsr { get; set; }
        public string wikidata { get; set; }
        public string google_entity_id { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
        public string official_full { get; set; }
        public string middle { get; set; }
        public string nickname { get; set; }
        public string suffix { get; set; }
    }

    public class Bio
    {
        public string birthday { get; set; }
        public string gender { get; set; }
    }

    public class Term
    {
        public string type { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string state { get; set; }
        public int district { get; set; }
        public string party { get; set; }
        public string url { get; set; }
        public int _class { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string contact_form { get; set; }
        public string office { get; set; }
        public string state_rank { get; set; }
        public string rss_url { get; set; }
        public string how { get; set; }
        public string caucus { get; set; }
        public Party_Affiliations[] party_affiliations { get; set; }
        public string endtype { get; set; }
    }

    public class Party_Affiliations
    {
        public string start { get; set; }
        public string end { get; set; }
        public string party { get; set; }
    }

    public class Leadership_Roles
    {
        public string title { get; set; }
        public string chamber { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Other_Names
    {
        public string last { get; set; }
    }

    public class Family
    {
        public string name { get; set; }
        public string relation { get; set; }
    }

}
