import {
  IconType,
  SiAmazonaws,
  SiCloudflare,
  SiDigitalocean,
  SiGooglecloud,
  SiMicrosoftazure,
  SiOracle,
} from '@icons-pack/react-simple-icons';
import styles from './CloudIcon.module.css';

export enum Cloud {
  Azure = 'Azure',
  AWS = 'AWS',
  GCP = 'GCP',
  Oracle = 'Oracle',
  DigitalOcean = 'DigitalOcean',
  Cloudflare = 'Cloudflare',
}

const cloudIcon = (cloudString: string): IconType | null => {
  if (!(cloudString in Cloud)) {
    return null;
  }

  const cloud = Cloud[cloudString as keyof typeof Cloud];

  switch (cloud) {
    case 'Azure':
      return SiMicrosoftazure;
    case 'AWS':
      return SiAmazonaws;
    case 'GCP':
      return SiGooglecloud;
    case 'Oracle':
      return SiOracle;
    case 'DigitalOcean':
      return SiDigitalocean;
    case 'Cloudflare':
      return SiCloudflare;
  }

  return null;
};

export const CloudIcon = ({ cloud }: { cloud: string }) => {
  const Icon = cloudIcon(cloud);

  if (Icon == null) {
    return;
  }

  return <Icon title={cloud} className={styles.cloudIcon} />;
};
