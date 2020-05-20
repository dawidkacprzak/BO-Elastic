﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.Panel.Exceptions
{
    public class VersionNotFoundException : Exception
    {
        public VersionNotFoundException() : base ("Nie znaleziono pliku z wersją aplikacji. Stwórz plik 'version' z zawartością '0' aby wymusić aktualizację.")
        {

        }
    }
}
