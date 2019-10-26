using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
    public static class ForgeExtensions
    {
        public static string Checkout(this string file)
        {
            if (!Provider.enabled || !Provider.hasCheckoutSupport)
            {
                //Do nothing if revision control is disabled, or if the revision system does not require checkouts
                return file;
            }
            
            AssetList list = new AssetList(); 
            list.Add(new Asset(file));

            if (File.Exists(file))
            {
                try
                {
                    Provider.Checkout(list, CheckoutMode.Asset).Wait();
                    Debug.Log("Checked out file: " + file);
                }
                catch (System.Exception)
                {
                    try
                    {
                        //Set it writable
                        File.SetAttributes(file, FileAttributes.Normal);
                        Debug.LogWarning("Set writable: " + file);
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("Can't checkout file: " + file);
                    }
                }
            }

            return file;
        }
    }
}