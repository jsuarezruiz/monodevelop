﻿//
// PackagingProject.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Projects;
using MonoDevelop.Projects.MSBuild;

namespace MonoDevelop.Packaging
{
	public class PackagingProject : DotNetProject
	{
		public PackagingProject ()
		{
			UsePartialTypes = false;
		}

		protected override DotNetCompilerParameters OnCreateCompilationParameters (
			DotNetProjectConfiguration config,
			ConfigurationKind kind)
		{
			return new PackagingCompilerParameters ();
		}

		protected override ClrVersion [] OnGetSupportedClrVersions ()
		{
			return new ClrVersion[] {
				ClrVersion.Net_4_5
			};
		}

		protected override bool OnSupportsFramework (TargetFramework framework)
		{
			bool result = base.OnSupportsFramework (framework);
			if (result)
				return result;

			if (framework.Id.Identifier == ".NETFramework") {
				Version version = null;
				if (System.Version.TryParse (framework.Id.Version, out version)) {
					return version.Major >= 4 && version.Minor >= 5;
				}
			}

			return false;
		}

		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configuration)
		{
			return false;
		}

		protected override void PopulateOutputFileList (
			List<FilePath> list,
			ConfigurationSelector configuration)
		{
			list.Add (OnGetOutputFileName (configuration));
		}

		protected override void OnPrepareForEvaluation (MSBuildProject project)
		{
			MSBuildPropertyGroup globalGroup = project.GetGlobalPropertyGroup ();
			var provider = new MSBuildGlobalPropertyProvider ();
			foreach (KeyValuePair<string, string> property in provider.GetGlobalProperties ()) {
				globalGroup.SetValue (property.Key, property.Value, property.Value);
			}
		}
	}
}

