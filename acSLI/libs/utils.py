#######################################################    
#
#    Author: Turnermator13
#
#    Please find this on github at:
#    -https://github.com/Turnermator13/ArduinoRacingDash
#
#######################################################

import ac
import os.path
import configparser

class Config:
    
    def __init__(self, filePath):
        self.file = filePath
        self.parser = 0
        
        if not os.path.isfile(self.file):
            open(self.file, 'a').close()
        
        try:
            self.parser = configparser.RawConfigParser()
        except:
            ac.console("Utils.Config: Failed to initialize ConfigParser.")
            
        self._read()
    
    
    
    # Private 
    def _read(self):
        self.parser.read(self.file)
    
    
    def _write(self):
        with open(self.file, "w") as cfgFile:
            self.parser.write(cfgFile)
            
    
    
    # Public
    def hasSection(self, section = None):
        if section is not None:
            return self.parser.has_section(section)
        else:
            return False
    
    
    def hasOption(self, section = None, option = None):
        if self.hasSection(section):
            if option is not None:
                return self.parser.has_option(section, option)
            else:
                return False
        else:
            return False
    
    def addSection(self, section = None):
        if section is not None:
            if not self.hasSection(section):
                self.parser.add_section(section)
                self._write()
                return True
            else:
                ac.console("Utils.Config.addSection -- Section '" + section + "' already exists.")
                return False
        
        
    def addOption(self, section = None, option = None, value = None):
        if not self.hasSection(section):
            self.addSection(section)
        
        if (option is not None) and (not self.hasOption(section, option)):
            if (value is not None):
                self.parser.set(section, option, value)
                self._write()
                return True
            else:
                ac.console("Utils.Config.addOption -- Value cannot be null")
                return False
        else:
            ac.console("Utils.Config.addOption -- Option '" + option + "' is blank or already exists in section '" + section + "'.")
            return False
    
    
    def updateOption(self, section = None, option = None, value = None, create = False):
        if self.hasOption(section, option):
            if value is not None:
                self.parser.set(section, option, value)
                self._write()
                return True
            else:
                ac.console("Utils.Config.updateOption -- Value cannot be null")
                return False
        else:
            if create:
                return self.addOption(section, option, value)
            else:
                ac.console("Utils.Config.updateOption -- Option '" + option + "' in section '" + section + "' doesn't exist.")
                return False
        
    
    def getOption(self, section, option, create = False, default = ""):
        if self.hasOption(section, option):
            return self.parser.get(section, option)
        else:
            if create:
                ac.console("Utils.Config.getOption -- Option '" + option + "' in section '" + section + "' doesn't exist. Creating with default value...")
                self.addOption(section, option, default)
                return default
            else:
                ac.console("Utils.Config.getOption -- Option '" + option + "' in section '" + section + "' doesn't exist.")
                return -1
        
        
    def removeSection(self, section):
        if self.hasSection(section):
            self.parser.remove_section(section)
            self._write()
            return True
        else:
            ac.console("Utils.Config.remSection -- section not found.")
            return False
        
        
    def removeOption(self, section, option):
        if self.hasOption(section,option):
            self.parser.remove_option(section, option)
            self._write()
            return True
        else:
            ac.console("Utils.Config.Config.remOption -- option not found.")
            return False 
