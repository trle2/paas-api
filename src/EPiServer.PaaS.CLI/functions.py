import argparse
from commands.prepare import PrepareArtifactCommand
from commands.build import BuildArtifactCommand
from commands.push import PushArtifactCommand
from commands.deploy import DeployArtifactCommand

class Functions:
	def __init__(self):
		self.prepareCommand=PrepareArtifactCommand()
		self.buildCommand=BuildArtifactCommand()
		self.pushCommand=PushArtifactCommand()
		self.deployCommand=DeployArtifactCommand()

	def populate(self):
		parser = argparse.ArgumentParser(description='EPiServer PaaS CLI.')
		subparsers = parser.add_subparsers()
		
		# generation
		generation_parser = subparsers.add_parser('generate', help='Generate artifact.')
		
		generation_parser.add_argument('-p', '--package', required=True, help='Package name for the deployment.')
		generation_parser.add_argument('-a', '--app_name', required=True, help='Application name.')
		generation_parser.add_argument('-l', '--app_location', required=True, help='csproj name(with relative path).')
		generation_parser.add_argument('-j', '--job_name', help='Jobs application name(if any).')
		generation_parser.add_argument('-o', '--job_location', help='csproj name of jobs application(with relative path).')
		generation_parser.set_defaults(func=self.prepareCommand.execute)
		
		# build
		build_parser = subparsers.add_parser('build', help='Build artifact.')
		build_parser.add_argument('-p', '--package', required=True, help='Package name for the deployment.')
		build_parser.set_defaults(func=self.buildCommand.execute)

		# push
		push_parser = subparsers.add_parser('push', help='Push artifact to hub.')
		push_parser.add_argument('-p', '--package', required=True, help='Package name for the deployment.')
		push_parser.set_defaults(func=self.pushCommand.execute)

		# deploy
		deploy_parser = subparsers.add_parser('deploy', help='Deploy the package.')
		deploy_parser.add_argument('-p', '--package', required=True, help='Package name for the deployment.')
		deploy_parser.set_defaults(func=self.deployCommand.execute)

		args = parser.parse_args()
		return args