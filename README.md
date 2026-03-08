
# API Integrada com IoT (ESP32) — Grupo 4

## Integrantes

* Davi Gualberto Leal
* Kauã Restoff de Oliveira
* João Pedro Pereira Gomes
* Pablo Miguel Oliveira da Silva
* Rúbia de Souza Soares Pereira

---
# Sumário

- [1. Introdução](#1-introdução)
- [2. Tecnologias Utilizadas](#2-tecnologias-utilizadas)
- [3. Como Executar a API](#3-como-executar-a-api)
  - [3.1 Configuração do Banco](#31-configuração-do-banco-de-dados)
  - [3.2 Criando as Tabelas](#32-criando-as-tabelas-do-banco)
  - [3.3 Executando a API](#33-executando-a-api-no-vs-code)
- [4. Integração com ESP32](#4-conectando-o-esp32-wokwi-com-a-api)
  - [4.1 Port Forwarding](#41-fazer-port-forwarding-no-vs-code)
  - [4.2 Tornar Porta Pública](#42-tornar-a-porta-pública)
  - [4.3 Conectar no Wokwi](#43-conectar-no-wokwi)
- [5. Funcionamento](#5-funcionamento)
---

# 1. Introdução

Este projeto consiste em uma **API desenvolvida em C# integrada a um dispositivo IoT (ESP32)**.

O ESP32 é simulado através da plataforma **Wokwi**, enviando dados para a API.
A API processa essas informações e as armazena em um banco de dados **MySQL**.

---

# 2. Tecnologias Utilizadas

* **C#**
* **ASP.NET Core**
* **MySQL**
* **XAMPP**
* **Wokwi (simulador de ESP32)**
* **VS Code / Visual Studio (Recomendado)**

---

# 3. Como Executar a API

Antes de iniciar o projeto, é necessário configurar o banco de dados e instalar as dependências.

---

# 3.1 Configuração do Banco de Dados

Antes de rodar a API, configure a conexão com o banco **MySQL**.

1. Abra o arquivo:

```
appsettings.json
```

2. Configure a **ConnectionString** com as credenciais do seu banco local.

```json
{
  "ConnectionStrings": {
    "ConexaoBanco": "Server=localhost;Database=estoquesa;Uid=root;Pwd='';"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Verifique se o nome da conexão (**ConexaoBanco**) está igual ao utilizado no `Program.cs`. 

**IMPORTANTE:** Geralmente no Visual Studio (recomendado), não é necessário mudar nada no ```appsettings.json``` 

---

# 3.2 Criando as Tabelas do Banco

Se estiver rodando o projeto pela primeira vez, é necessário aplicar as **migrations** para criar as tabelas no banco de dados.

No terminal, execute:

```bash
dotnet ef database update
```

Caso der algum erro, coloque o banco de dados **MySQL** abaixo:
```SQL
-- Active: 1772829380216@@localhost@3306@estoquesa
CREATE DATABASE IF NOT EXISTS EstoqueSA;

USE EstoqueSA;

CREATE TABLE IF NOT EXISTS Perfil (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Usuario (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    login VARCHAR(255) NOT NULL UNIQUE,
    senhaHash VARCHAR(255) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT true,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    perfilId INT NOT NULL,
    CONSTRAINT Usuario_perfilId_fkey FOREIGN KEY (perfilId) REFERENCES Perfil (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS EventoUsuario (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    usuarioId INT,
    tipoEvento VARCHAR(255) NOT NULL,
    descricao TEXT,
    ip VARCHAR(45),
    dataEvento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT EventoUsuario_usuarioId_fkey FOREIGN KEY (usuarioId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE IF NOT EXISTS Produto (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Estoque (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    produtoId INT NOT NULL,
    quantidade INT NOT NULL DEFAULT 0,
    estoqueMinimo INT NOT NULL DEFAULT 0,
    unidadeMedida VARCHAR(50),
    CONSTRAINT Estoque_produtoId_fkey FOREIGN KEY (produtoId) REFERENCES Produto (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Movimentacao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    estoqueId INT NOT NULL,
    usuarioId INT,
    tipo VARCHAR(50) NOT NULL,
    quantidade INT NOT NULL,
    dataMovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Movimentacao_estoqueId_fkey FOREIGN KEY (estoqueId) REFERENCES Estoque (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT Movimentacao_usuarioId_fkey FOREIGN KEY (usuarioId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE IF NOT EXISTS EtapaProducao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nomeEtapa VARCHAR(100) NOT NULL,
    descricao VARCHAR(500),
    ordem INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS OrdemProducao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    numeroOp INT NOT NULL UNIQUE,
    produtoId INT NOT NULL,
    quantidade INT NOT NULL,
    status VARCHAR(50),
    CONSTRAINT OrdemProducao_produtoId_fkey FOREIGN KEY (produtoId) REFERENCES Produto (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS EtapaOP (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    opId INT NOT NULL,
    etapaId INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pendente',
    dataInicio DATETIME NULL,
    dataFim DATETIME NULL,
    CONSTRAINT EtapaOP_opId_fkey FOREIGN KEY (opId) REFERENCES OrdemProducao (id) ON DELETE CASCADE,
    CONSTRAINT EtapaOP_etapaId_fkey FOREIGN KEY (etapaId) REFERENCES EtapaProducao (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE IF NOT EXISTS Equipamento (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    setor VARCHAR(100),
    status VARCHAR(50)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Manutencao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    equipamentoId INT NOT NULL,
    tipo VARCHAR(100) NOT NULL,
    descricao TEXT,
    dataManutencao DATETIME,
    responsavelId INT,
    CONSTRAINT Manutencao_equipamentoId_fkey FOREIGN KEY (equipamentoId) REFERENCES Equipamento (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT Manutencao_responsavelId_fkey FOREIGN KEY (responsavelId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Indicador (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    equipamentoId INT NOT NULL,
    temperatura DOUBLE,
    vibracao DOUBLE,
    horasUso INT,
    dataRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Indicador_equipamentoId_fkey FOREIGN KEY (equipamentoId) REFERENCES Equipamento (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS sensores (
    id_sensor INT PRIMARY KEY AUTO_INCREMENT,
    nome_equipamento VARCHAR(100), 
    tipo_sensor VARCHAR(50), 
    localizacao VARCHAR(100),
    status_ativo BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS alerta (
    id_alerta INT PRIMARY KEY AUTO_INCREMENT,
    id_sensor INT,
    tipo_alerta VARCHAR(50),
    valor_detectado DECIMAL(10,2), 
    limite_configurado DECIMAL(10,2),
    mensagem TEXT,
    data_alerta DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (id_sensor) REFERENCES sensores(id_sensor)
);


CREATE TABLE IF NOT EXISTS leituras (
    id_leitura INT PRIMARY KEY AUTO_INCREMENT,
    id_sensor INT,
    tipo_leitura VARCHAR(50), 
    valor DECIMAL(10,2),       
    unidade VARCHAR(20),      
    descricao TEXT,   
    data_leitura DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (id_sensor) REFERENCES sensores(id_sensor)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO sensores (id_sensor, nome_equipamento, tipo_sensor, localizacao, status_ativo)
VALUES 
(1, 'Motor Principal', 'Temperatura', 'Setor A', 1),
(2, 'Ambiente Fabrica', 'Umidade', 'Setor B', 1),
(3, 'Eixo do Torno CNC', 'Vibracao', 'Setor C', 1);
```
**IMPORTANTE:** Certifique-se de que o **XAMPP** está funcionando; se está com MySQL verdinho e Apache também para checar o phpmyadmin.

---

# 3.3 Executando a API no VS Code

Abra o terminal integrado do **VS Code**.

Atalho:

```
Ctrl + `
```

Depois execute o comando:

```bash
dotnet run
```

A API será iniciada localmente.

Exemplo de endereço gerado:

```
http://localhost:5050
```

Fique atento à **porta exibida no terminal**, pois ela será utilizada na integração com o ESP32.

---

# 4. Conectando o ESP32 (Wokwi) com a API

Como o **Wokwi roda na nuvem** e a API roda **localmente**, é necessário expor a porta da aplicação para acesso externo.

Para conseguir criar o API de forma correta, abra o site **https://wokwi.com/**, selecione **ESP32** e por fim **Wifi Scanning**

após isso, no **wifi-scan.ino** coloque o seguinte código abaixo:

```C++
#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "DHT.h"

const char* ssid = "Wokwi-GUEST";
const char* password = "";

const char* serverName = "https://qgsxkcn5-5050.brs.devtunnels.ms/api/Sensor";

// Configuração dos Pinos
#define DHTPIN 15
#define DHTTYPE DHT22
DHT dht(DHTPIN, DHTTYPE);

#define POTPIN 34

void setup() {
  Serial.begin(115200);
  dht.begin();
  
  // Conectando na internet do Wokwi
  WiFi.begin(ssid, password);
  Serial.print("Conectando ao WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\n✅ Conectado com sucesso!");
}

// Função que empacota o JSON e manda pra API
void enviarLeitura(int idSensor, String tipo, float valor, String unidade, String descricao) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    http.begin(serverName);
    http.addHeader("Content-Type", "application/json");

    StaticJsonDocument<200> doc;
    doc["id_sensor"] = idSensor;
    doc["tipo_leitura"] = tipo;
    doc["valor"] = valor;
    doc["unidade"] = unidade;
    doc["descricao"] = descricao;

    String requestBody;
    serializeJson(doc, requestBody);

    // mandando o POST
    int httpResponseCode = http.POST(requestBody);

    // Mostrando o resultado no terminal do VS Code
    Serial.print("Sensor ID " + String(idSensor) + " [" + tipo + "] -> Resposta API: ");
    Serial.println(httpResponseCode);
    
    http.end();
  } else {
    Serial.println("❌ Erro: WiFi desconectado.");
  }
}

void loop() {
  Serial.println("\n--- Iniciando nova leitura dos sensores ---");
  
  // Lendo Temperatura e Umidade do DHT22
  float temp = dht.readTemperature();
  float umidade = dht.readHumidity();
  
  // Lendo o potenciômetro e transformando num valor de "vibração" (de 0.0 a 5.0)
  int valorPot = analogRead(POTPIN);
  float vibracao = (valorPot / 4095.0) * 5.0; 

  // Checa se o DHT22 não falhou antes de enviar
  if (!isnan(temp) && !isnan(umidade)) {
    
    enviarLeitura(1, "Temperatura", temp, "°C", "Motor Principal");
    delay(1000); // Pausa de 1s
    
    enviarLeitura(2, "Umidade", umidade, "%", "Ambiente Fabrica");
    delay(1000);
    
    enviarLeitura(3, "Vibracao", vibracao, "mm/s", "Eixo do Torno CNC");
  } else {
    Serial.println("⚠️ Falha ao ler o DHT22!");
  }
  
  // pausa 10 segundos
  delay(10000); 
}
```
logo após no arquivo **diagram.json** cole o código abaixo:

```JSON
{
  "version": 1,
  "author": "Equipe 4",
  "editor": "wokwi",
  "parts": [
    { "type": "board-esp32-devkit-c-v4", "id": "esp", "top": -124.8, "left": -23.96, "attrs": {} },
    { "type": "wokwi-dht22", "id": "dht1", "top": -306.9, "left": 4.2, "attrs": {} },
    { "type": "wokwi-potentiometer", "id": "pot1", "top": 46.7, "left": 153.4, "attrs": {} }
  ],
  "connections": [
    [ "esp:15", "dht1:SDA", "green", [ "h48", "v-240" ] ],
    [ "esp:3V3", "dht1:VCC", "red", [ "v19.2", "h-47.85", "v-115.2" ] ],
    [ "esp:GND", "dht1:GND", "black", [ "v0" ] ],
    [ "esp:3V3", "pot1:VCC", "red", [ "v28.8", "h-105.45", "v201.6", "h327.2" ] ],
    [ "esp:GND", "pot1:GND", "black", [ "v0" ] ],
    [ "esp:34", "pot1:SIG", "yellow", [ "h-47.85", "v211.2", "h259.6" ] ]
  ],
  "dependencies": {}
}
```
Após isso, crie um arquivo chamado **libraries.txt** e cole o texto abaixo:
```TXT
# Wokwi Library List
# See https://docs.wokwi.com/guides/libraries

# Automatically added based on includes:
DHT sensor library

ArduinoJson

Adafruit Unified Sensor
```

Certifique-se que irá aparecer os mesmos nomes inseridos no arquivo anterior no arquivo **Library Manager** como na imagem abaixo
<img width="807" height="174" alt="image" src="https://github.com/user-attachments/assets/7ac33946-b716-4cd7-830a-8931f059bc17" />

---

## 4.1 Fazer Port Forwarding no VS Code

1. Com a API rodando, abra a aba:

```
Ports
```

Ela fica ao lado da aba **Terminal** no painel inferior do VS Code.

2. Clique em:

```
Forward a Port
```

3. Digite a porta da API.

Exemplo:

```
5050
```

---

## 4.2 Tornar a Porta Pública

Após criar o Port Forward:

1. Clique com o botão direito na porta criada
2. Vá em:

```
Port Visibility
```

3. Altere de:

```
Private → Public
```

---

## 4.3 Conectar no Wokwi

1. Copie o **Forwarded Address** gerado pelo VS Code.

Exemplo:

```
https://aaaaaaa1-5000.brs.devtunnels.ms/
```

2. Cole esse endereço no código do **ESP32 no Wokwi**, adicionando a rota do controller, linha ```const char* serverName = "https://qgsxkcn5-5050.brs.devtunnels.ms/api/Sensor";```.

Exemplo:

```
https://aaaaaaa1-5000.brs.devtunnels.ms/api/Sensor
```

---

# 5. Funcionamento

1. O **ESP32 (Wokwi)** coleta ou simula dados de sensores
2. Esses dados são enviados para a **API**
3. A API processa a requisição
4. Os dados são armazenados no **MySQL**

---

Pronto. Agora o **ESP32 do Wokwi consegue enviar dados para a API e armazená-los no banco local**.

---
