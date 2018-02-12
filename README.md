** This project is no longer actively maintained. Use at own risk. **

MailChimpConnector
==================

Subscribe / Unsbscribe Forms for MailChimp in DNN made easy. This template based module allows you letting your site visitors subscribe / unsubscribe from and to mailing list on MailChimp.

All you need is a MailChimp account, an API key for that account and at least one list.

The forms rendered to the client are template / tokens based. That gives you full flexibility in setting up your subscription forms.

You can reference all merge fields from your maling list in the templates. For instance, if you want to know if your subscribers code in C# or VB you would add a merge field in mailChimp called "Preferred Coding Language", that would then get a merge name like 'PREFCOLA' and from within the subscriptin form template you would referene that field like so:

    <div class="dnnFormItem">
      [MERGE:PREFCOLA:LABEL]
      [MERGE:PREFCOLA:CONTROL]
      [MERGE:PREFCOLA:REQUIRED]
    </div>
    
Since you have setup the merge field in MailChimp as a select field, this would then render as a dropdownlist in your module's form.

Now there is another option too. You can also make the form dead simple and only display a message and a subscribe button an let the module still sync the user's profile data to MailChimp. You would then create a custom profile property in DNN
and call it 'PREFCOLA'. So what the module does, is if there is no form field for a given merge field in MailChimp it will lookup if there is a matching profile property in the user's profile.

Make sure then though that the merge field is not required or, make sure that each of your users have filled in the property field in DNN!

Have a look the two templates that are being delivered with the module. That should give you an idea what can be done.
