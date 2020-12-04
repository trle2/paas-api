import docker
import requests
from configurations import Configuration

class BuildArtifactCommand:
	def __init__(self):
		config=Configuration()
		self.configuration=config.get_configuration()

	def execute(self, args):
		url=self.configuration["ApiBaseUrl"] + '/deployment/' + args.package
		result = requests.get(url, verify=False)

		docker_api = docker.APIClient()
		for project in result.json():
			tag = self.configuration["DockerServerName"] + '/' + project
			dockerfile = project + 'Dockerfile'
			for line in docker_api.build(path='.', tag=tag, rm=True, dockerfile=dockerfile):
				print(line)