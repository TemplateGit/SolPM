﻿using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SolPM.Core.Models;
using SolPM.Core.ViewModels.Parameters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SolPM.Core.ViewModels
{
    public class OpenVaultViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;

        public OpenVaultViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;

            // Commands

            NavigateVaultView = new MvxAsyncCommand(() => _navigationService.Navigate<VaultViewModel>());
            OpenVaultCommand = new MvxCommand<VaultParams>((s) => OpenVault(s), (s) => CanOpenVault(s));
        }

        public override async Task Initialize()
        {
        }

        #region Commands

        public IMvxAsyncCommand NavigateVaultView { get; private set; }
        public IMvxCommand OpenVaultCommand { get; private set; }

        #endregion Commands

        private void OpenVault(VaultParams vaultParams)
        {
            System.Diagnostics.Debug.WriteLine($"-- OpenVault -> vaultParams.FilePath = {vaultParams.FilePath}");

            if (Vault.Exists())
            {
                // TODO: Figure out what to do with this
                var _vault = Vault.GetInstance();
                _vault.EncryptToFile(_vault.Location, _vault.EncryptionInfo.ProtectedKey);
                Vault.Delete();
            }

            if (string.IsNullOrWhiteSpace(vaultParams.FilePath))
            {
                throw new ArgumentNullException("filePath", "Path to file is either null or empty.");
            }

            var vault = Vault.GetInstance();

            vault.DecryptFromFile(vaultParams.FilePath, vaultParams.Password);
            vault.SetupProtectedKey(vaultParams.Password);

            _navigationService.Navigate<VaultViewModel>();
        }

        public bool CanOpenVault(VaultParams vaultParams)
        {
            if (null == vaultParams)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(vaultParams.FilePath))
            {
                return false;
            }

            if (!File.Exists(vaultParams.FilePath))
            {
                return false;
            }

            if (null == vaultParams.Password || vaultParams.Password.Length <= 0)
            {
                return false;
            }

            return true;
        }
    }
}