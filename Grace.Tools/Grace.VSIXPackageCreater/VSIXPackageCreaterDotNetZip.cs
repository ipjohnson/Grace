using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace Grace.VSIXPackageCreater
{
	public class VSIXPackageCreaterDotNetZip
	{
		private string packageName;
		private string versionNumber;
		private string packagePath;
		private string outputPath;
		private List<string> packageList = new List<string>();

		public VSIXPackageCreaterDotNetZip(string versionNumber, string packagePath, string outputPath)
		{
			this.versionNumber = versionNumber;
			this.packagePath = packagePath;
			this.outputPath = outputPath;
		}

		public void CreatePackage()
		{
			if (File.Exists(outputPath))
			{
				File.Delete(outputPath);
			}

			using (ZipFile archive = new ZipFile(outputPath))
			{
				AddManifest(archive);

				AddLicense(archive);

				AddContentType(archive);

				AddPackages(archive);

				AddItemTemplates(archive);

				AddProjectTemplates(archive);

				archive.Save();
			}
		}

		private void AddContentType(ZipFile archive)
		{
			byte[] contentBytes = File.ReadAllBytes("[Content_Types].xml");

			archive.AddEntry("[Content_Types].xml", contentBytes);
		}

		private void AddLicense(ZipFile archive)
		{
			byte[] licenseBytes = File.ReadAllBytes(packagePath + @"\License.rtf");

			archive.AddEntry("License.rtf", licenseBytes);
		}

		private void AddManifest(ZipFile archive)
		{
			byte[] manifestBytes = File.ReadAllBytes(packagePath + @"\source.extension.vsixmanifest");

			manifestBytes = UpdateManifest(manifestBytes);

			archive.AddEntry("extension.vsixmanifest", manifestBytes);
		}

		private byte[] UpdateManifest(byte[] manifestBytes)
		{
			StringBuilder returnFile = new StringBuilder();
			StreamReader reader = new StreamReader(new MemoryStream(manifestBytes));
			string currentLine;

			while(!reader.EndOfStream)
			{
				string addString = currentLine = reader.ReadLine();
				string trimmed = currentLine.Trim();

				if (trimmed.StartsWith("<Identity "))
				{
					int versionIndex = currentLine.IndexOf("Version=");

					if (versionIndex > 0)
					{
						int firstQuote = currentLine.IndexOf('"', versionIndex);
						int secondQuote = currentLine.IndexOf('"', firstQuote + 1);

						addString = currentLine.Substring(0, firstQuote + 1);
						addString += versionNumber;
						addString += currentLine.Substring(secondQuote);
					}
				}

				returnFile.AppendLine(addString);
			}
			return UTF8Encoding.UTF8.GetBytes(returnFile.ToString());
		}

		private void AddPackages(ZipFile archive)
		{
			foreach (string file in Directory.GetFiles(packagePath + @"\packages\", "*.nupkg"))
			{
				byte[] fileBytes = File.ReadAllBytes(file);

				packageList.Add(Path.GetFileNameWithoutExtension(file));

				archive.AddEntry("packages\\" + Path.GetFileName(file), fileBytes);
			}
		}

		private void AddProjectTemplates(ZipFile archive)
		{
			TemplateDirectory(archive, packagePath + "\\ProjectTemplates", "ProjectTemplates");
		}

		private void TemplateDirectory(ZipFile archive, string currentProjectDirectory, string zipPath)
		{
			foreach (string directory in Directory.GetDirectories(currentProjectDirectory))
			{
				string parentDirectoryName = Path.GetDirectoryName(directory);

				string directoryName = directory.Remove(0, parentDirectoryName.Length + 1);

				directoryName = directoryName.Replace(" ", "%20");

				TemplateDirectory(archive, directory, zipPath + directoryName);
			}

			foreach (string file in Directory.GetFiles(currentProjectDirectory))
			{
				if (!file.EndsWith(".zip"))
				{
					continue;
				}

				byte[] fileBytes = File.ReadAllBytes(file);

				fileBytes = UpdateTemplate(fileBytes);

				string fileName = Path.GetFileName(file);

				fileName = fileName.Replace(" ", "%20");

				archive.AddEntry(zipPath + "\\" + fileName, fileBytes);
			}
		}


		private byte[] UpdateTemplate(byte[] templateBytes)
		{
			using (MemoryStream zipStream = new MemoryStream(templateBytes) { Position = 0 })
			{
				using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
				{
					using (MemoryStream returnStream = new MemoryStream())
					{
						using (ZipArchive returnArchive = new ZipArchive(returnStream,ZipArchiveMode.Create))
						{
							foreach (ZipArchiveEntry zipArchiveEntry in archive.Entries)
							{
								MemoryStream fullFile = new MemoryStream();

								using (Stream readStream = zipArchiveEntry.Open())
								{
									byte[] buffer = new byte[8192];
									int readLength;

									while ((readLength = readStream.Read(buffer, 0, buffer.Length)) > 0)
									{
										fullFile.Write(buffer, 0, readLength);
									}
								}

								ZipArchiveEntry newEntry = returnArchive.CreateEntry(zipArchiveEntry.FullName);

								byte[] totalBytes = null;

								if (zipArchiveEntry.FullName.EndsWith(".vstemplate"))
								{
									fullFile.Position = 0;

									totalBytes = ReplaceVersionString(fullFile);
								}
								else
								{
									totalBytes = fullFile.ToArray();
								}

								using (Stream openNewStream = newEntry.Open())
								{
									openNewStream.Write(totalBytes, 0, totalBytes.Length);
								}
							}
						}

						return returnStream.ToArray();
					}
				}
			}
		}

		private byte[] ReplaceVersionString(MemoryStream fullFile)
		{
			TextReader reader = new StreamReader(fullFile);
			StringBuilder returnStringBuilder = new StringBuilder();

			string readLine;
			while ((readLine = reader.ReadLine()) != null)
			{
				int index = readLine.IndexOf("<package id=");

				if (index > 0)
				{
					int firstQuote = readLine.IndexOf('"');
					int secondQuote = readLine.IndexOf('"', firstQuote + 1);

					string packageName = readLine.Substring(firstQuote + 1, (secondQuote - firstQuote) - 1);
					string foundPackageVersion = null;

					foreach (string testPackage in packageList)
					{
						if (testPackage.StartsWith(packageName))
						{
							string versionString = testPackage.Substring(packageName.Length + 1);

							if (char.IsDigit(versionString[0]))
							{
								foundPackageVersion = versionString;
								break;
							}
						}
					}

					if (!string.IsNullOrEmpty(foundPackageVersion))
					{
						returnStringBuilder.Append(readLine.Substring(0, index));

						returnStringBuilder.AppendFormat("<package id=\"{0}\" version=\"{1}\" />{2}",
							packageName,
							foundPackageVersion,
							Environment.NewLine);
					}
					else
					{
						returnStringBuilder.AppendLine(readLine);
					}
				}
				else
				{
					returnStringBuilder.AppendLine(readLine);
				}
			}

			return Encoding.UTF8.GetBytes(returnStringBuilder.ToString());
		}

		private void AddItemTemplates(ZipFile archive)
		{

		}
	}
}
