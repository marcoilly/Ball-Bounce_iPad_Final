// TODO:
// By default, screenshot files are placed next to the executable bundle -- we don't want this in a
// shipping game, as it will fail if the user doesn't have write access to the Applications folder.
// Instead we should place the screenshots on the user's desktop. However, the ~/ notation doesn't
// work, and Unity doesn't have a mechanism to return special paths. Therefore, the correct way to
// solve this is probably with a plug-in to return OS specific special paths.

// Mono/.NET has functions to get special paths... see discussion page. --Aarku

using UnityEngine;
using System.Collections;

public class TakeScreenShot : MonoBehaviour
{    
	private int screenshotCount = 0;
	
	// Check for screenshot key each frame
	void Update()
	{
		// take screenshot on up->down transition of F9 key
		if (Input.GetKeyDown("e"))
		{        
			string screenshotFilename;
			do
			{
				screenshotCount++;
				screenshotFilename = "screenshot" + screenshotCount + ".png";
				
			} while (System.IO.File.Exists(screenshotFilename));
			
			if (Screen.width == 414) {
				ScreenCapture.CaptureScreenshot (screenshotFilename, 3);
				
			} else if (Screen.width == 512) {
				ScreenCapture.CaptureScreenshot (screenshotFilename, 4);
			}
			else
			{
				ScreenCapture.CaptureScreenshot(screenshotFilename,1);
				
			}
			
		}
	}
}