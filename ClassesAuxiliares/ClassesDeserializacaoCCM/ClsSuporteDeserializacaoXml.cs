using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;

public class ClsSuporteDeserializacaoXml : XmlTextReader
{

    public ClsSuporteDeserializacaoXml(TextReader reader) : base(reader) { }

    public override string NamespaceURI
    {
        get { return ""; }
    }
}
