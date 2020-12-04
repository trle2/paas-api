import requests
from configurations import Configuration

class DeployArtifactCommand:
	def __init__(self):
		config=Configuration()
		self.configuration=config.get_configuration()

	def execute(self, args):
		url=self.configuration["ApiBaseUrl"] + '/deployment/' + args.package
		result = requests.put(url,verify=False)
		print(result.json())
		result_obj=result.json()
		print('Successfully deploying package: ' + result_obj["Package"])
		print('Deployed artifacts:')
		for project in result_obj["Projects"]:
			print(project + '.southeastasia.azurecontainer.io')
