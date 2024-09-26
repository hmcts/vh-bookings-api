## Configuration

The primary difference between running locally and running in a container is the configuration. Devs primarily use the `dotnet secrets` tool to manage secrets locally. Though we could mount a volume to load the secrets, this is not reflective of how the app behaves when built with Release configuration. The app, when running in a container on AKS expects secrets to be loaded from both environment variables and Azure Key Vault (stored in files in a fixed folder structure).

What this looks like is that the app will look for a file in the following location: `/mnt/<secrets-store>/` where `<secret-store> `is the name of each key vault the app depends on.

Since writing to the root directory is not allowed by most operatin systems as a measure to protect the core of the OS, a volume must be mounted to the container to allow the app to read the secrets.

### Initialising secrets

Make the script executable

```bash
chmod +x setup-secrets.sh
```

Run the script

```bash
./setup-secrets.sh
```

As mentioned above, it is not guaranteed to be able to write to the root directory of the OS, so the secrets will be written to the user home directory (`~/mnt/`) instead.
