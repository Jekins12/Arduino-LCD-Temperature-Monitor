#include <Wire.h> 
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27,16,2);
String inData;

void setup() {
    Serial.begin(9600);
    lcd.init();
    lcd.backlight();
    lcd.setCursor(0,0);
    lcd.print("Waiting for");
    lcd.setCursor(0,1);
    lcd.print("connection...");
}

void loop() {
    
    
    
    while (Serial.available() > 0)
    {
        char recieved = Serial.read();
        inData += recieved; 
        
        if (recieved == '*')
        {
            lcd.clear();
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(0,0);
            lcd.print("GPU: " + inData + char(223)+"C ");
             
            
            if(inData == "DIS")
            {   
              lcd.clear();
              lcd.setCursor(0,0);
              lcd.print("Disconnected!");
              delay(2000);
              lcd.clear();
              lcd.setCursor(0,0);
              lcd.print("Waiting for");
              lcd.setCursor(0,1);
              lcd.print("connection...");
            }
            inData = "";
        } 

        if (recieved == '$')
        {
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(10,0);
            lcd.print(inData + char(37));
            inData = ""; 
        }
        
        if (recieved == '#')
        {
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(0,1);
            lcd.print("CPU: " + inData + char(223)+"C ");
            inData = ""; 
        }

        if (recieved == '%')
        {
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(10,1);
            lcd.print(inData + char(37));
            inData = ""; 
        }

        if (recieved == 't')
        {
            lcd.clear();
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(0,0);
            lcd.print(inData);
            inData = ""; 
        }

        if (recieved == 'd')
        {
            inData.remove(inData.length() - 1, 1);
            lcd.setCursor(0,1);
            lcd.print(inData);
            inData = ""; 
        }

        
    }
    
    
    
    
}
