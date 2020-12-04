import os

class Configuration:
    def get_configuration(self):
        client_id = os.environ["AZURE_CLIENT_ID"]
        client_secret = os.environ["AZURE_CLIENT_SECRET"]
        docker_server_name = os.environ["AZURE_ACR_SERVER"]

        return  {
            "ClientId": client_id,
            "ClientSecret": client_secret,
            "DockerServerName": docker_server_name,
            "ApiBaseUrl": "https://localhost:32778"
        }