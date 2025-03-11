from socket import *
import json

# Opret forbindelse til serveren
host = "localhost"
port = 42010
clientSocket = socket(AF_INET, SOCK_STREAM)
clientSocket.connect((host, port))
print(f"Forbundet til serveren på port {port}")


while True:
    method = input("Vælg metode (Random, Add, Subtract). Tast 'q' for at afslutte: ")
    if method.lower() == 'q':
        print("Afslutter klienten.")
        break

    tal1_str = input("Første tal: ")
    tal2_str = input("Andet tal: ")

    # Forsøg at parse tallene som heltal
    try:
        tal1 = int(tal1_str)
        tal2 = int(tal2_str)
    except ValueError:
        print("Fejl: Tallet skal være et heltal. Prøv igen.")
        continue

    # Byg JSON-objekt
    requestObj = {
        "Method": method,
        "Tal1": tal1,
        "Tal2": tal2
    }

    # Konverter til JSON og send + newline
    requestJson = json.dumps(requestObj)
    clientSocket.sendall((requestJson + "\n").encode("utf-8"))

    # Modtag svar fra server (JSON)
    responseBytes = clientSocket.recv(4096)
    if not responseBytes:
        print("Serveren lukkede forbindelsen.")
        break

    # Decode og parse JSON
    responseStr = responseBytes.decode("utf-8-sig").strip()
    try:
        responseObj = json.loads(responseStr)
    except json.JSONDecodeError as e:
        print("Fejl: Kunne ikke parse serverens JSON:", e)
        break

    # Se om serveren returnerede Error eller Result
    if "Error" in responseObj:
        print(f"Fejl fra server: {responseObj['Error']}")
    else:
        res = responseObj.get("Result", None)
        print(f"Resultat: {res} (Status: {responseObj.get('Status','?')})")

# Luk forbindelsen, når brugeren forlader løkken
clientSocket.close()
print("Lukker klienten.")
