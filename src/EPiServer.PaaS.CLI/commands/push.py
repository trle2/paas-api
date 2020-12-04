import docker
import requests
from configurations import Configuration

class PushArtifactCommand:
	def __init__(self):
		config=Configuration()
		self.configuration=config.get_configuration()

	def execute(self, args):
		url=self.configuration["ApiBaseUrl"] + '/deployment/' + args.package
		result = requests.get(url, verify=False)

		docker_api = docker.APIClient()
		for project in result.json():
			tag = self.configuration["DockerServerName"] + '/' + project
			for line in docker_api.push(repository=tag, stream=True,auth_config={'username':self.configuration["ClientId"],'password':self.configuration["ClientSecret"]}):
				print(line)