import requests
from configurations import Configuration

class PrepareArtifactCommand:
	def __init__(self):
		config=Configuration()
		self.configuration=config.get_configuration()

	def execute(self, args):
		self.prepareAppArtifact(args)
		self.prepareJobsAppArtifact(args)

	def prepareAppArtifact(self, args):
		url=self.configuration["ApiBaseUrl"] + '/preparation?deploymentPackageName=' + args.package + '&projectName=' + args.app_name +  '&projectLocation=' + args.app_location + '&type=1'
		result = requests.get(url,verify=False)
		output=args.app_name + 'Dockerfile'
		open(output, 'wb').write(result.content)
		return output

	def prepareJobsAppArtifact(self, args):
		url=self.configuration["ApiBaseUrl"] + '/preparation?deploymentPackageName=' + args.package + '&projectName=' + args.job_name +  '&projectLocation=' + args.job_location + '&type=2'
		result = requests.get(url,verify=False)
		output=args.job_name + 'Dockerfile'
		open(output, 'wb').write(result.content)
		return output